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

        private List<Vector2Int> _validMovesCache = new();

        private List<Vector2Int> GetValidMoves(Vector2Int currentPos)
        {
            _validMovesCache.Clear();
            switch (_moveSet)
            {
                case MoveSet.Orthogonal:
                {
                    GetValidMovesOrthogonal(_validMovesCache, currentPos);
                    break;
                }
                case MoveSet.Diagonal:
                {
                    Moves.GetValidMovesDiagonal(_validMovesCache, _grid, currentPos);
                    break;
                }
                default:
                {
                    Assert.IsTrue(false, "Fail");
                    return null!;
                }
            }
            return _validMovesCache;
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

        private void GetValidMovesOrthogonal(List<Vector2Int> result, Vector2Int playerPos)
        {
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
        public static void GetValidMovesDiagonal(
            List<Vector2Int> result,
            Grid grid,
            Vector2Int playerPos)
        {
            Vector2Int currentDir = new(1, 1);
            var p = new GetCellInDirectionParams(
                result,
                grid,
                StartPos: playerPos,
                Dir: currentDir);
            GetCellsInDirectionRotated(p);
        }

        public static void GetCellsInDirectionRotated(GetCellInDirectionParams p)
        {
            var currentDir = p.Dir;
            for (int i = 0; i < 4; i++)
            {
                GetCellsInDirection(p with
                {
                    Dir = currentDir,
                });

                currentDir = new(currentDir.y, -currentDir.x);
            }
        }

        public static void GetCellsInDirection(GetCellInDirectionParams p)
        {
            Vector2Int currentPos = p.StartPos;
            while (true)
            {
                currentPos += p.Dir;
                if (!p.Grid.IsInGrid(currentPos))
                {
                    break;
                }

                p.Result.Add(currentPos);
            }
        }
    }

    public readonly record struct GetCellInDirectionParams(
        List<Vector2Int> Result,
        Grid Grid,
        Vector2Int StartPos,
        Vector2Int Dir);
}
