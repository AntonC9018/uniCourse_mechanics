using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Core
{
    public enum MoveSet
    {
        Orthogonal,
        Diagonal,
    }

    public sealed class Player : MonoBehaviour
    {
        [SerializeField] private CellTargeting _cellTargeting = null!;
        [SerializeField] private Grid _grid = null!;
        [SerializeField] private CellHighlighting _cellHigh = null!;
        [SerializeField] private Color _availableMoveColor = Color.coral;
        private MoveSet _moveSet = MoveSet.Orthogonal;

        private void Start()
        {
            HighlightMoves();
        }

        private void Update()
        {
            SwitchMoveSet();
            DoPlayerMouseMovement();
        }

        private void SwitchMoveSet()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (_moveSet == MoveSet.Orthogonal)
                {
                    _moveSet = MoveSet.Diagonal;
                }
                else
                {
                    _moveSet = MoveSet.Orthogonal;
                }

                HighlightMoves();
            }
        }

        private void HighlightMoves()
        {
            // find available moves
            var availableMovesInGridSpace = FindAvailableMoves();

            var layer = _cellHigh.ModifyLayer(HighlightLayer.AvailableMoves);
            layer.SetColor(_availableMoveColor);
            layer.Clear();
            foreach (var h in availableMovesInGridSpace)
            {
               layer.AddHigh(h);
            }
        }

        private List<Vector2Int> FindAvailableMoves()
        {
            var playerPosInGridSpace = _grid.WorldToGridOfCellOrigin(transform.position);
            var ret = GetValidMoves(playerPosInGridSpace);
            return ret;
        }

        private void DoPlayerMouseMovement()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            var cellPositionGridSpace = _cellTargeting.FindCellPositionUnderMouse();
            if (!_grid.IsInGrid(cellPositionGridSpace))
            {
                return;
            }

            var t = transform;

            var playerPosInGridSpace = _grid.WorldToGridOfCellOrigin(t.position);
            if (!CheckValidMove(playerPosInGridSpace, cellPositionGridSpace))
            {
                return;
            }

            Vector3 playerNewWorldPos = _grid.GridToWorld(cellPositionGridSpace);
            playerNewWorldPos.z = t.position.z;
            t.position = playerNewWorldPos;

            HighlightMoves();
        }

        private bool CheckValidMove(Vector2Int currentPos, Vector2Int newPos)
        {
            switch (_moveSet)
            {
                case MoveSet.Orthogonal:
                {
                    return CheckValidMoveOrthogonal(currentPos, newPos);
                }
                case MoveSet.Diagonal:
                {
                    return CheckValidMoveDiagonal(currentPos, newPos);
                }
                default:
                {
                    Assert.IsTrue(false, "Fail");
                    return false;
                }
            }
        }

        private List<Vector2Int> GetValidMoves(Vector2Int currentPos)
        {
            switch (_moveSet)
            {
                case MoveSet.Orthogonal:
                {
                    return GetValidMovesOrthogonal(currentPos);
                }
                case MoveSet.Diagonal:
                {
                    return Moves.GetValidMovesDiagonal(_grid, currentPos);
                }
                default:
                {
                    Assert.IsTrue(false, "Fail");
                    return null!;
                }
            }
        }

        private bool CheckValidMoveOrthogonal(Vector2Int currentPos, Vector2Int newPos)
        {
            bool x = currentPos.x == newPos.x;
            bool y = currentPos.y == newPos.y;
            if (x || y)
            {
                return true;
            }
            return false;
        }

        private List<Vector2Int> GetValidMovesOrthogonal(Vector2Int playerPos)
        {
            var result = new List<Vector2Int>();

            var size = _grid.Size;
            for (int x = 0; x < size.x; x++)
            {
                if (x == playerPos.x)
                {
                    continue;
                }

                result.Add(new(x: x, y: playerPos.y));
            }
            for (int y = 0; y < size.y; y++)
            {
                if (y == playerPos.y)
                {
                    continue;
                }

                result.Add(new(y: y, x: playerPos.x));
            }

            return result;
        }

        private bool CheckValidMoveDiagonal(Vector2Int currentPos, Vector2Int newPos)
        {
            if (currentPos == newPos)
            {
                return false;
            }
            var diff = currentPos - newPos;
            if (Math.Abs(diff.x) == Math.Abs(diff.y))
            {
                return true;
            }
            return false;
        }
    }

    public static class Moves
    {
        public static List<Vector2Int> GetValidMovesDiagonal(Grid grid, Vector2Int playerPos)
        {
            var result = new List<Vector2Int>();

            Vector2Int currentDir = new(1, 1);
            for (int i = 0; i < 4; i++)
            {
                Vector2Int currentPos = playerPos;
                while (true)
                {
                    currentPos += currentDir;
                    if (!grid.IsInGrid(currentPos))
                    {
                        break;
                    }

                    result.Add(currentPos);
                }

                currentDir = new(currentDir.y, -currentDir.x);
            }
            return result;
        }
    }
}
