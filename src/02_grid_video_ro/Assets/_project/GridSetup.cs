#if UNITY_EDITOR
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace Core
{
    public sealed class GridSetup : MonoBehaviour
    {
        [SerializeField]
        private Transform _gridParent = null!;

        [Button]
        private void SetupGrid()
        {
            Undo.RegisterFullObjectHierarchyUndo(_gridParent, "Delete old grid objects");

            int childCount = _gridParent.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                var child = _gridParent.GetChild(i);
                Object.DestroyImmediate(child.gameObject);
            }

            // dorim ca obiectele sa se deseneze
            // camera nu este positionata
            const int width = 5;
            const int height = 10;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2 gridPos = new(x, y);
                    var worldPos = GridToWorld(gridPos);
                    var cell = new GameObject();
                    cell.name = $"x={gridPos.x},y={gridPos.y}";

                    Vector3 worldPos3 = worldPos;
                    worldPos3.z = 0;

                    var ct = cell.transform;
                    ct.position = worldPos3;

                    Undo.SetTransformParent(ct, _gridParent, "Set cell parent");
                    Undo.RegisterCreatedObjectUndo(cell, $"Create cell at {cell.name}");
                }
            }
        }

        private Vector2 GridToWorld(Vector2 gridPos)
        {
            Vector2 ret;
            ret.x = gridPos.x;
            ret.y = -gridPos.y;
            return ret;
        }
    }
}
#endif