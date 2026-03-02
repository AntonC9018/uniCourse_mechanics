using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Core
{
    public sealed class Player : MonoBehaviour
    {
        [SerializeField] private CellTargeting _cellTargeting = null!;
        [SerializeField] private Grid _grid = null!;
        [SerializeField] private Color _availableMoveColor = Color.coral;
        private List<SpriteRenderer> _highlighted = new();

        private void Start()
        {
            HighlightMoves();
        }

        private void Update()
        {
            DoPlayerMouseMovement();
        }

        private void HighlightMoves()
        {
            // find available moves
            var availableMovesInGridSpace = FindAvailableMoves();
            // moves -> cell
            var cellsToHighlight = new Transform[availableMovesInGridSpace.Count];
            for (int index = 0; index < cellsToHighlight.Length; index++)
            {
                var pos = availableMovesInGridSpace[index];
                var cell = _grid.FindCellAt(pos);
                Assert.IsNotNull(cell);
                cellsToHighlight[index] = cell!;
            }
            // dehighlight
            foreach (var h in _highlighted)
            {
                h.color = Color.white;
            }
            _highlighted.Clear();

            // highlight
            foreach (var c in cellsToHighlight)
            {
                var renderer1 = CellHelper.GetSpriteRenderer(c.gameObject);
                renderer1.color = _availableMoveColor;
                _highlighted.Add(renderer1);
            }
        }

        private List<Vector2Int> FindAvailableMoves()
        {
            var playerPosInGridSpace = _grid.WorldToGridOfCellOrigin(transform.position);

            var result = new List<Vector2Int>();

            var size = _grid.Size;
            for (int x = 0; x < size.x; x++)
            {
                if (x == playerPosInGridSpace.x)
                {
                    continue;
                }

                result.Add(new(x: x, y: playerPosInGridSpace.y));
            }
            for (int y = 0; y < size.y; y++)
            {
                if (y == playerPosInGridSpace.y)
                {
                    continue;
                }

                result.Add(new(y: y, x: playerPosInGridSpace.x));
            }

            return result;
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
            if (playerPosInGridSpace.x != cellPositionGridSpace.x
                && playerPosInGridSpace.y != cellPositionGridSpace.y)
            {
                return;
            }

            Vector3 playerNewWorldPos = _grid.GridToWorld(cellPositionGridSpace);
            playerNewWorldPos.z = t.position.z;
            t.position = playerNewWorldPos;

            HighlightMoves();
        }
    }
}
