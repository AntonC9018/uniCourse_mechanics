using UnityEngine;

namespace Core
{
    public sealed class CellTargeting : MonoBehaviour
    {
        [SerializeField] private Grid _grid = null!;
        [SerializeField] private Camera _camera = null!;

        public Transform? FindCellUnderMouse()
        {
            var cellPosition = FindCellPositionUnderMouse();
            var cell = _grid.FindCellAt(cellPosition);
            return cell;
        }

        public Vector2Int FindCellPositionUnderMouse()
        {
            var mousePosInScreenSpace = Input.mousePosition;
            var mousePosInWorldSpace = _camera.ScreenToWorldPoint(mousePosInScreenSpace);
            var mousePosInGridSpace = _grid.WorldToGrid(mousePosInWorldSpace);
            var cellPosition = _grid.SnapToCellOrigin(mousePosInGridSpace);
            return cellPosition;
        }
    }
}