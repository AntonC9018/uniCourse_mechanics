using UnityEngine;

namespace Core
{
    [DefaultExecutionOrder(HighController.ExecutionOrder - 1)]
    public sealed class CellHoverHigh : MonoBehaviour
    {
        [SerializeField] private Grid _grid = null!;
        [SerializeField] private CellTargeting _cellTargeting = null!;
        [SerializeField] private HighController _highController = null!;

        void Update()
        {
            var group = _highController.Group(HighGroup.Hover);

            var cellPosInGridSpace = _cellTargeting.GetCellPositionOfMouse();
            if (!_grid.IsInGrid(cellPosInGridSpace))
            {
                group.Clear();
                return;
            }
            group.SetColor(Color.green);
            group.ResetHigh(cellPosInGridSpace);
        }
    }
}