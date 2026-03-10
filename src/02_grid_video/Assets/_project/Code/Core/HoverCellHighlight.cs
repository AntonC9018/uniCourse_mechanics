using UnityEngine;

namespace Core
{
    public sealed class HoverCellHighlight : MonoBehaviour
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

            var layer = _cellHigh.ModifyLayer(HighlightLayer.Hover);
            layer.Clear();
            if (!_grid.IsInGrid(cellPosGridSpace))
            {
                return;
            }

            layer.SetColor(_highlightColor);
            layer.AddHigh(cellPosGridSpace);
        }
    }
}
