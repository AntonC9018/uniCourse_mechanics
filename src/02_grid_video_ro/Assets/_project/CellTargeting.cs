using UnityEngine;

namespace Core
{
    public sealed class CellTargeting : MonoBehaviour
    {
        [SerializeField] private Grid _grid = null!;
        [SerializeField] private Camera _camera = null!;

        public Vector2Int GetCellPositionOfMouse()
        {
            var mousePosInScreenSpace = Input.mousePosition;
            var mousePosInWorldSpace = _camera.ScreenToWorldPoint(mousePosInScreenSpace);
            var mousePosInGridSpace = _grid.WorldToGrid(mousePosInWorldSpace);
            var cellPosInGridSpace = _grid.SnapToCellOrigin(mousePosInGridSpace);
            return cellPosInGridSpace;
        }
    }
}