using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Core
{
    public enum HighGroup
    {
        ValidMoves = 0,
        Hover = 1,
        Count,
    }

    [DefaultExecutionOrder(ExecutionOrder)]
    public sealed class HighController : MonoBehaviour
    {
        public const int ExecutionOrder = 0;

        [SerializeField] private Grid _grid = null!;

        private readonly List<SpriteRenderer> _highCells = new();
        private readonly HighGroupData[] _data = CreateData();
        private bool _dirty = false;

        private static HighGroupData[] CreateData()
        {
            var arr = new HighGroupData[(int) HighGroup.Count];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = HighGroupData.Create();
            }
            return arr;
        }

        public readonly struct HighGroupController
        {
            private readonly HighController _impl;
            private readonly HighGroup _id;

            public HighGroupController(
                HighController impl,
                HighGroup id)
            {
                _impl = impl;
                _id = id;
            }

            private ref HighGroupData Data => ref _impl.GroupData(_id);

            public void Clear()
            {
                if (Data.Positions.Count == 0)
                {
                    return;
                }
                Data.Positions.Clear();
                _impl._dirty = true;
            }

            public void SetColor(Color color)
            {
                if (color == Data.Color)
                {
                    return;
                }
                Data.Color = color;
                _impl._dirty = true;
            }

            public void High(List<Vector2Int> positions)
            {
                Data.Positions.AddRange(positions);
                _impl._dirty = true;
            }

            public void ResetHigh(Vector2Int position)
            {
                var p = Data.Positions;
                var impl = _impl;
                if (p.Count != 1)
                {
                    p.Clear();
                    Set();
                }
                else if (p[0] != position)
                {
                    Set();
                }

                void Set()
                {
                    p.Add(position);
                    impl._dirty = true;
                }
            }
        }

        public HighGroupController Group(HighGroup id)
        {
            return new(this, id);
        }

        public ref HighGroupData GroupData(HighGroup id)
        {
            return ref _data[(int) id];
        }

        private void Update()
        {
            if (!_dirty)
            {
                return;
            }
            foreach (var h in _highCells)
            {
                h.color = Color.white;
            }
            _highCells.Clear();

            foreach (var d in _data)
            {
                Apply(d);
            }
            _dirty = false;
        }

        private void Apply(HighGroupData data)
        {
            foreach (var pos in data.Positions)
            {
                var cell = _grid.FindCellAt(pos);
                if (cell is null)
                {
                    continue;
                }
                var spriteRenderer = CellHelper.GetSpriteRenderer(cell.gameObject);
                spriteRenderer.color = data.Color;
                _highCells.Add(spriteRenderer);
            }
        }
    }

    public struct HighGroupData
    {
        public List<Vector2Int> Positions;
        public Color Color;

        public static HighGroupData Create()
        {
            return new()
            {
                Positions = new(),
                Color = Color.darkViolet,
            };
        }
    }
}