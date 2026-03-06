using UnityEngine;

namespace Core
{
    public sealed class Player : MonoBehaviour
    {
        [SerializeField] private CellTargeting _cellTargeting = null!;
        [SerializeField] private Grid _grid = null!;

        // unde si cand am apasat mouse-ul
        // gasim pozitia jucatorului
        // gasim vectorul de deplasare (target - player pos) in grid space
        // verificam daca ori x ori y ii 0

        private void Update()
        {
            const int left = 0;
            if (!Input.GetMouseButtonDown(left))
            {
                return;
            }

            var targetPosInGridSpace = _cellTargeting.GetCellPositionOfMouse();
            if (!_grid.IsInGrid(targetPosInGridSpace))
            {
                return;
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
            }
        }
    }
}