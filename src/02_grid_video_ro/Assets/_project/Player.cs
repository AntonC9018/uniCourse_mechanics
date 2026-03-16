using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [DefaultExecutionOrder(HighController.ExecutionOrder - 1)]
    public sealed class Player : MonoBehaviour
    {
        [SerializeField] private CellTargeting _cellTargeting = null!;
        [SerializeField] private Grid _grid = null!;
        [SerializeField] private HighController _highController = null!;
        private List<Vector2Int> _validMoves = new();

        private void Start()
        {
            GetValidMoves(_validMoves);
            HighValidMoves();
        }

        private void Update()
        {
            if (TryMove())
            {
                GetValidMoves(_validMoves);
                HighValidMoves();
            }
        }

        private void HighValidMoves()
        {
            var group = _highController.Group(HighGroup.ValidMoves);
            group.Clear();
            group.SetColor(Color.crimson);
            group.High(_validMoves);
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