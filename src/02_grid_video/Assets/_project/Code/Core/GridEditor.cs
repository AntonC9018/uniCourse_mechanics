#if UNITY_EDITOR
using System;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace Core
{
    public sealed class GridEditor : MonoBehaviour
    {
        [SerializeField] private Transform _parent = null!;
        [SerializeField] private GameObject _cellPrefab = null!;
        [SerializeField] private Sprite[] _cellSprites = Array.Empty<Sprite>();
        [SerializeField] private Grid _grid = null!;

        [Range(0, 1)]
        [SerializeField]
        private float _cellSpriteChangeFactor = 10;

        [SerializeField]
        private Camera _camera = null!;

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
        private void ResetCellSprites()
        {
            Undo.RegisterCompleteObjectUndo(this, nameof(ResetCellSprites));
            const string path = "Assets/_project/Tileset/all-rock-tiles.png";
            var assets = AssetDatabase.LoadAllAssetsAtPath(path);
            _cellSprites = assets.OfType<Sprite>().ToArray();
        }

        [Button]
        private void CreateGrid()
        {
            var parent = _parent;

            Undo.RegisterFullObjectHierarchyUndo(_parent, "Delete old cells");

            int childCount = _parent.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                var child = _parent.GetChild(i);
                DestroyImmediate(child.gameObject);
            }

            var size = _grid.Size;
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    var gridPos = new Vector2(x, y);
                    var worldPos = _grid.GridToWorld(gridPos);

                    var cell = Instantiate(_cellPrefab);
                    cell.name = $"x={x}, y={y}";

                    var renderer_ = CellHelper.GetSpriteRenderer(cell);
                    float spriteValue = Mathf.PerlinNoise(x * _cellSpriteChangeFactor, y * _cellSpriteChangeFactor);
                    int spriteIndex = Mathf.FloorToInt(spriteValue * _cellSprites.Length);
                    spriteIndex = Math.Clamp(spriteIndex, 0, _cellSprites.Length - 1);

                    renderer_.sprite = _cellSprites[spriteIndex];

                    Vector3 pos;
                    pos.x = worldPos.x;
                    pos.y = worldPos.y;
                    pos.z = 0;

                    var cellt = cell.transform;
                    cellt.position = pos;
                    Undo.RegisterCreatedObjectUndo(cell, $"Create cell {x} {y}");
                    Undo.SetTransformParent(cellt, parent, "Parent the cell");
                }
            }

            SetupCamera();
        }

        private void SetupCamera()
        {
            var size = _grid.Size;
            var gridCenter = ((Vector2) size) / 2;
            Vector3 gridCenterInWorldSpace = _grid.GridToWorld(gridCenter);
            gridCenterInWorldSpace.z = -1;

            var ct = _camera.transform;
            Undo.RecordObject(ct, "Reset camera position");
            ct.position = gridCenterInWorldSpace;


            Undo.RecordObject(_camera, "Projection");
            _camera.orthographic = true;
            _camera.orthographicSize = (float) size.y / 2;
            _camera.nearClipPlane = -1;
            _camera.farClipPlane = 1;
        }
    }
}
#endif