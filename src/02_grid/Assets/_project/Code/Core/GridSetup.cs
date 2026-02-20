#if UNITY_EDITOR
using System;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using Random = System.Random;
using UnityEngine.Assertions;

namespace Core.Editor
{
    public sealed class GridSetup : MonoBehaviour
    {
        [SerializeField] private GameObject? _cellPrefab;
        [SerializeField] private Sprite[] _backgroundSprites = Array.Empty<Sprite>();
        [SerializeField] private Camera? _camera;
        [SerializeField] private Grid? _grid;

        [MenuItem("Grid/Create GridSetup")]
        public static void CreateGridSetup()
        {
            using var undo = new UndoGroup(nameof(CreateGridSetup));
            var setup = new GameObject();
            setup.name = "setup";
            setup.hideFlags = HideFlags.DontSaveInBuild;
            setup.tag = "EditorOnly";
            Undo.RegisterCreatedObjectUndo(setup, "Create");
            Undo.AddComponent<GridSetup>(setup);
        }

        private bool CanSetupHighlight => _grid != null && _camera != null;

        [Button]
        [EnableIf(nameof(CanSetupHighlight))]
        private void SetupHighlight()
        {
            using var undo = new UndoGroup(nameof(SetupHighlight));

            var x = _grid!.gameObject.GetOrAddComponent<CellHighlight>();
            Undo.RecordObject(x, nameof(SetupHighlight));
            x.Setup(_grid!, _camera!);
        }

        [Button]
        private void SetGridObject()
        {
            using var undo = new UndoGroup(nameof(SetGridObject));
            Undo.RecordObject(this, nameof(GridSetup));

            GameObject gridRoot;
            if (_grid != null)
            {
                gridRoot = _grid.gameObject;
            }
            else
            {
                var x = new GameObject();
                Undo.RegisterCreatedObjectUndo(x, "Grid");
                var parent = transform.parent;

                var xt = x.transform;
                Undo.SetTransformParent(xt, parent, "Parent");
                gridRoot = x;
            }
            Undo.RecordObject(gridRoot, "Grid changes");
            gridRoot.name = "grid";

            if (_grid == null)
            {
                var x = gridRoot.GetOrAddComponent<Grid>();
                _grid = x;
            }
        }

        [Button]
        private void RefreshCamera()
        {
            Undo.RecordObject(this, nameof(RefreshCamera));
            var t = Camera.main;
            Assert.IsNotNull(t);
            _camera = t;
        }

        [Button]
        private void RefreshBackgroundSprites()
        {
            Undo.RecordObject(this, nameof(RefreshBackgroundSprites));
            var sprites = AssetDatabase.LoadAllAssetsAtPath("Assets/_project/Tileset/all-rock-tiles.png");
            _backgroundSprites = sprites.OfType<Sprite>().ToArray();
        }

        [Button]
        private void CreateCellPrefab()
        {
            var parent = new GameObject();
            var child = new GameObject();
            child.transform.parent = parent.transform;
            var lp = GridHelper.GetWorldOffsetToCorner(GridHelper.CellSize);
            child.transform.localPosition = -lp;
            _ = child.AddComponent<SpriteRenderer>();
            var prefab = PrefabUtility.SaveAsPrefabAsset(parent, "Assets/grid_cell.prefab");
            _cellPrefab = prefab;
            DestroyImmediate(parent);
        }

        private bool CanRefreshCells => CanSetupCamera && CanInitGrid;

        [Button]
        [EnableIf(nameof(CanRefreshCells))]
        private void RefreshCells()
        {
            InitGrid(_grid!.Size);
            SetupCamera();
        }

        private bool CanSetupCamera => _camera != null && _grid != null;

        [Button]
        [EnableIf(nameof(CanSetupCamera))]
        private void SetupCamera()
        {
            using var undo = new UndoGroup(nameof(SetupCamera));

            {
                Vector2 v = _grid!.Size.Value;
                Vector2 gridCenter = v / 2;
                Vector2 worldGridCenter = GridHelper.GridToWorldPosition(_grid.Size, gridCenter);
                Vector3 v3 = worldGridCenter;
                v3.z = -1;
                Undo.RecordObject(_camera!.transform, "Transform");
                _camera!.transform.position = v3;
            }
            {
                Undo.RecordObject(_camera, "Projection");
                _camera.orthographicSize = ((float) _grid.Size.Y) / 2;
                _camera.orthographic = true;
                _camera.nearClipPlane = 0;
                _camera.farClipPlane = 2;
            }
        }

        private bool CanInitGrid =>
            _cellPrefab != null
            && _grid != null
            && _backgroundSprites.Length != 0;

        private void InitGrid(GridSize size)
        {
            using var undo = new UndoGroup(nameof(InitGrid));

            var root = _grid!.transform;

            var children = root.OfType<Transform>().ToArray();
            foreach (var c in children)
            {
                Undo.DestroyObjectImmediate(c.gameObject);
            }

            var seed = 10;
            var rng = new Random(seed);

            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    Vector2Int gridPos = new(x, y);
                    Vector2 worldPos = GridHelper.GridToWorldPosition(size, gridPos);
                    var cell = Instantiate(
                        _cellPrefab!,
                        parent: root,
                        position: worldPos,
                        rotation: Quaternion.identity);
                    cell.name = $"{x},{y}";

                    var r = cell.GetComponentInChildren<SpriteRenderer>();
                    Assert.IsNotNull(r);

                    // upper bound is exclusive.
                    int index = rng.Next(_backgroundSprites.Length);
                    r.sprite = _backgroundSprites[index];

                    Undo.RegisterCreatedObjectUndo(cell.gameObject, $"Create {cell.name}");
                }
            }
        }
    }
}
#endif