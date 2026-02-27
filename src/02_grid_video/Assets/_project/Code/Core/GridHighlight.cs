using UnityEngine;

namespace Core
{
    public sealed class GridHighlight : MonoBehaviour
    {
        [SerializeField] private Grid _grid = null!;
        [SerializeField] private Camera _camera = null!;
        [SerializeField] private Color _highlightColor = Color.green;

        private void Update()
        {
            Highlight();
        }

        private void Highlight()
        {
            var mousePosInScreenSpace = Input.mousePosition;
            var mousePosInWorldSpace = _camera.ScreenToWorldPoint(mousePosInScreenSpace);
            var mousePosInGridSpace = _grid.WorldToGrid(mousePosInWorldSpace);
            var cellPosition = _grid.SnapToCellOrigin(mousePosInGridSpace);
            var cell = _grid.FindCellAt(cellPosition);
            if (cell == null)
            {
                return;
            }
            var spriteRenderer = CellHelper.GetSpriteRenderer(cell.gameObject);
            spriteRenderer.color = _highlightColor;
        }
    }
}
