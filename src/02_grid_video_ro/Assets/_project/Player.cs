using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

namespace Core
{
    public sealed class Player : MonoBehaviour
    {
        [SerializeField] private CellTargeting _cellTargeting = null!;
        [SerializeField] private Grid _grid = null!;

        private List<SpriteRenderer> _highCells = new();
        private List<Vector2Int> _validMoves = new();

        private void Start()
        {
            GetValidMoves(_validMoves);
        }

        private void Update()
        {
            if (TryMove())
            {
                GetValidMoves(_validMoves);
            }
            HighValidMoves();
        }

        private void HighValidMoves()
        {
            foreach (var h in _highCells)
            {
                h.color = Color.white;
            }
            _highCells.Clear();

            var validMoves = _validMoves;
            foreach (var m in validMoves)
            {
                var cell = _grid.FindCellAt(m);
                Assert.IsNotNull(cell);

                var r = CellHelper.GetSpriteRenderer(cell!.gameObject);
                r.color = Color.crimson;

                _highCells.Add(r);
            }
        }

        private void GetValidMoves(List<Vector2Int> list)
        {
            list.Clear();

            var playerTransform = transform;
            var playerGridSpace = _grid.WorldToGrid(playerTransform.position);
            var playerGridSpaceInt = _grid.MakeSureCellOrigin(playerGridSpace);

            Vector2Int dir = new(1, 0);
            for (int i = 0; i < 4; i++)
            {
                var currentPos = playerGridSpaceInt;
                while (true)
                {
                    currentPos += dir;
                    if (!_grid.IsInGrid(currentPos))
                    {
                        break;
                    }
                    list.Add(currentPos);
                }

                dir = new(x: -dir.y, y: dir.x);
            }
        }

        private bool TryMove()
        {
            const int left = 0;
            if (!Input.GetMouseButtonDown(left))
            {
                return false;
            }

            var targetPosInGridSpace = _cellTargeting.GetCellPositionOfMouse();
            if (!_grid.IsInGrid(targetPosInGridSpace))
            {
                return false;
            }

            var playerTransform = transform;
            var playerGridSpace = _grid.WorldToGrid(playerTransform.position);
            var playerGridSpaceInt = _grid.MakeSureCellOrigin(playerGridSpace);
            var diff = targetPosInGridSpace - playerGridSpaceInt;
            if (diff.x == 0 || diff.y == 0)
            {
                var pos = playerTransform.position;
                Vector3 playerNewPosInWorldSpace = _grid.GridToWorld(targetPosInGridSpace);
                playerNewPosInWorldSpace.z = pos.z;
                playerTransform.position = playerNewPosInWorldSpace;
                return true;
            }
            return false;
        }
    }
}