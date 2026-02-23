#if UNITY_EDITOR
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace Core
{
    public sealed class GridEditor : MonoBehaviour
    {
        [MenuItem("!!!STUFF!!!/Create Setup Object", isValidateFunction: false, priority = 100)]
        public static void CreateSetupObject()
        {
            var setup = new GameObject();
            setup.name = "setup";
            setup.tag = "EditorOnly";
            var editor = setup.AddComponent<GridEditor>();
            _ = editor;
            Undo.RegisterCreatedObjectUndo(setup, nameof(CreateSetupObject));
        }

        [Button]
        private void CreateGrid()
        {
            var parent = transform;

            const int width = 4;
            const int height = 4;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var gridPos = new Vector2(x, y);
                    var worldPos = GridToWorld(gridPos);

                    var cell = new GameObject();
                    cell.name = $"x={x}, y={y}";

                    Vector3 pos;
                    pos.x = worldPos.x;
                    pos.y = worldPos.y;
                    pos.z = 1;

                    var cellt = cell.transform;
                    cellt.position = pos;
                    cellt.SetParent(parent);
                }
            }
        }

        private static Vector2 GridToWorld(Vector2 gridPos)
        {
            Vector2 result;
            result.x = gridPos.x;
            result.y = -gridPos.y;
            return result;
        }
    }
}
#endif