#if UNITY_EDITOR
using System;
using System.Linq;
using System.Numerics;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Core
{
    public sealed class GridSetup : MonoBehaviour
    {
        [SerializeField]
        private Transform _gridParent = null!;

        [SerializeField]
        private GameObject _cellPrefab = null!;

        [SerializeField]
        private Camera _camera = null!;

        [SerializeField]
        private Sprite[] _cellSprites = null!;

        [SerializeField]
        private Texture2D _spritesheet = null!;

        [SerializeField]
        [Range(0, 1)]
        private float _spriteChangeRate = 0.1f;

        [SerializeField]
        private Vector2 _spriteChangeSeed = Vector2.zero;

        const int width = 5;
        const int height = 10;

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

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2 gridPos = new(x, y);
                    var worldPos = GridToWorld(gridPos);

                    Vector3 worldPos3 = worldPos;
                    worldPos3.z = 0;

                    var cell = GameObject.Instantiate(
                        _cellPrefab,
                        parent: _gridParent,
                        position: worldPos3,
                        rotation: Quaternion.identity);
                    cell.name = $"x={gridPos.x},y={gridPos.y}";

                    var perlinCoords = _spriteChangeRate * gridPos + _spriteChangeSeed;
                    float spriteValue = Mathf.PerlinNoise(perlinCoords.x, perlinCoords.y);
                    float spriteIndexValue = spriteValue * _cellSprites.Length;
                    int spriteIndex = Mathf.FloorToInt(spriteIndexValue);
                    spriteIndex = Math.Clamp(spriteIndex, 0, _cellSprites.Length - 1);

                    var sprite = _cellSprites[spriteIndex];
                    var renderer1 = cell.GetComponentInChildren<SpriteRenderer>();
                    renderer1.sprite = sprite;

                    // var ct = cell.transform;
                    // Undo.SetTransformParent(ct, _gridParent, "Set cell parent");
                    Undo.RegisterCreatedObjectUndo(cell, $"Create cell at {cell.name}");
                }
            }
        }

        [Button]
        private void SetupCamera()
        {
            {
                Vector2 gridCenterInGridSpace = new(width, height);
                gridCenterInGridSpace /= 2;
                Vector2 gridCenterInWorldSpace = GridToWorld(gridCenterInGridSpace);

                var t = _camera.transform;
                Undo.RegisterCompleteObjectUndo(t, nameof(SetupCamera));
                t.position = gridCenterInWorldSpace;
            }
            {
                Undo.RegisterCompleteObjectUndo(_camera, "Change projection");

                _camera.orthographic = true;
                _camera.orthographicSize = (float) height / 2;
                _camera.nearClipPlane = -1;
                _camera.farClipPlane = 1;
            }
        }

        [Button]
        private void LoadSprites()
        {
            var spritesheetPath = AssetDatabase.GetAssetPath(_spritesheet);
            var objects = AssetDatabase.LoadAllAssetsAtPath(spritesheetPath);
            var sprites = objects.OfType<Sprite>().ToArray();
            Undo.RegisterCompleteObjectUndo(this, nameof(LoadSprites));
            _cellSprites = sprites;
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