using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Core
{
    public enum HighlightLayer
    {
        AvailableMoves = 0,
        Hover = 1,
        Count = 2,
    }

    public readonly struct LayerAccessor
    {
        private readonly CellHighlighting _impl;
        private readonly HighlightLayer _layer;

        public LayerAccessor(HighlightLayer layer, CellHighlighting impl)
        {
            _layer = layer;
            _impl = impl;
        }

        public void SetColor(Color color) => _impl.SetColor(_layer, color);
        public void AddHigh(Vector2Int cellPos) => _impl.AddHigh(_layer, cellPos);
        public void RemoveHigh(Vector2Int cellPos) => _impl.RemoveHigh(_layer, cellPos);
        public void Clear() => _impl.Clear(_layer);
    }

    [DefaultExecutionOrder(ExecutionOrder)]
    public sealed class CellHighlighting : MonoBehaviour
    {
        public const int ExecutionOrder = 300;

        [SerializeField] private Grid _grid = null!;
        private List<SpriteRenderer> _highlighted = new();
        private List<Vector2Int>[] _layers = CreateLayers();
        private Color[] _colors = new Color[(int) HighlightLayer.Count];

        private List<Vector2Int> Layer(HighlightLayer layer)
        {
            var ret = _layers[(int) layer];
            return ret;
        }

        public LayerAccessor ModifyLayer(HighlightLayer layer) => new(layer, this);

        public void SetColor(HighlightLayer layer, Color color)
        {
            _colors[(int) layer] = color;
        }

        public void AddHigh(HighlightLayer layer, Vector2Int cellPos)
        {
            Layer(layer).Add(cellPos);
        }

        public void RemoveHigh(HighlightLayer layer, Vector2Int cellPos)
        {
            Layer(layer).Remove(cellPos);
        }

        public void Clear(HighlightLayer layer)
        {
            Layer(layer).Clear();
        }

        private void Update()
        {
            foreach (var h in _highlighted)
            {
                Assert.IsNotNull(h);
                h.color = Color.white;
            }
            _highlighted.Clear();

            for (int layerIndex = 0; layerIndex < _layers.Length; layerIndex++)
            {
                var layer = _layers[layerIndex];
                var color = _colors[layerIndex];
                foreach (var pos in layer)
                {
                    var cell = _grid.FindCellAt(pos);
                    Assert.IsNotNull(cell);

                    var r = CellHelper.GetSpriteRenderer(cell!.gameObject);
                    r.color = color;

                    _highlighted.Add(r);
                }
            }
        }

        private static List<Vector2Int>[] CreateLayers()
        {
            var ret = new List<Vector2Int>[(int) HighlightLayer.Count];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = new();
            }
            return ret;
        }
    }
}
