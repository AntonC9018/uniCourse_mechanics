using UnityEngine;

namespace Core
{
    public sealed class GridHighlight : MonoBehaviour
    {
        [SerializeField] private CellTargeting _cellTargeting = null!;
        [SerializeField] private Grid _grid = null!;
        [SerializeField] private Color _highlightColor = Color.green;
        [SerializeField] private CellHighlighting _cellHigh = null!;

        private void Update()
        {
            Highlight();
        }

        private void Highlight()
        {
            var cellPosGridSpace = _cellTargeting.FindCellPositionUnderMouse();
            _cellHigh.Clear(HighlightLayer.Hover);
            if (!_grid.IsInGrid(cellPosGridSpace))
            {
                return;
            }

            _cellHigh.SetColor(HighlightLayer.Hover, _highlightColor);
            _cellHigh.AddHigh(cellPosGridSpace, HighlightLayer.Hover);
        }
    }
}
