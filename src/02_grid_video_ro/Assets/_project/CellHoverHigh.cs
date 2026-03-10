using UnityEngine;

namespace Core
{
    public sealed class CellHoverHigh : MonoBehaviour
    {
        [SerializeField] private Grid _grid = null!;
        [SerializeField] private CellTargeting _cellTargeting = null!;

        private SpriteRenderer? _highlightedCell = null;

        void Update()
        {
            if (_highlightedCell != null)
            {
                _highlightedCell.color = Color.white;
            }

            var cellPosInGridSpace = _cellTargeting.GetCellPositionOfMouse();
            if (!_grid.IsInGrid(cellPosInGridSpace))
            {
                return;
            }
            var cell = _grid.FindCellAt(cellPosInGridSpace);
            if (cell == null)
            {
                return;
            }

            var renderer1 = CellHelper.GetSpriteRenderer(cell.gameObject);
            renderer1.color = Color.green;
            _highlightedCell = renderer1;
        }
    }
}