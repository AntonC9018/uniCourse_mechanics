using UnityEngine;

namespace Core
{
    public sealed class CellHoverHigh : MonoBehaviour
    {
        [SerializeField] private Grid _grid = null!;
        [SerializeField] private Camera _camera = null!;

        private SpriteRenderer? _highlightedCell = null;

        void Update()
        {
            if (_highlightedCell != null)
            {
                _highlightedCell.color = Color.white;
            }

            var mousePosInScreenSpace = Input.mousePosition;
            var mousePosInWorldSpace = _camera.ScreenToWorldPoint(mousePosInScreenSpace);
            var mousePosInGridSpace = _grid.WorldToGrid(mousePosInWorldSpace);
            var cellPosInGridSpace = _grid.SnapToCellOrigin(mousePosInGridSpace);
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