using System;
using System.Linq;
using UnityEngine;

namespace Core
{
    public sealed class Grid : MonoBehaviour
    {
        const int defaultWidth = 4;
        const int defaultHeight = 6;

        [SerializeField]
        private GridSize _size = new(defaultWidth, defaultHeight);

        public GridSize Size => _size;

        public Transform? FindCell(Vector2Int pos)
        {
            foreach (var cell in transform.Cast<Transform>())
            {
                var centerPos = ((Vector2) cell.localPosition);
                var worldPos = centerPos;
                var gridPos = GridHelper.WorldToGridPosition(_size, worldPos);
                var gridIntPos = GridHelper.SnapToInt(gridPos);
                Debug.Assert(((Vector2) gridIntPos) == (gridPos), "Position of cell itself is whole");

                if (gridIntPos == pos)
                {
                    return cell;
                }
            }
            return null;
        }
    }

    [Serializable]
    public struct GridSize
    {
        [SerializeField]
        [Min(1)]
        private int _x;

        [SerializeField]
        [Min(1)]
        private int _y;

        public int X
        {
            get => _x;
            set
            {
                Debug.Assert(value >= 1);
                _x = value;
            }
        }
        public int Y
        {
            get => _y;
            set
            {
                Debug.Assert(value >= 1);
                _y = value;
            }
        }
        public Vector2Int Value => new(X, Y);

        public GridSize(int x, int y)
        {
            this = default;
            X = x;
            Y = y;
        }
    }

    public static class GridHelper
    {
        public static Vector2 CellSize => new(1, 1);

        public static Vector2 GetWorldOffsetToCorner(Vector2 size)
        {
            // top-left corner.
            var ret = new Vector2();
            ret.x = -size.x / 2;
            ret.y = +size.y / 2;
            return ret;
        }
        public static Vector2 GridToWorldPosition(GridSize gridSize, Vector2 gridPos)
        {
            float x = gridPos.x;
            float y = gridSize.Y - 1 - gridPos.y;
            return new(x, y);
        }
        public static Vector2 WorldToGridPosition(GridSize gridSize, Vector2 worldPos)
        {
            float x = worldPos.x;
            float y = gridSize.Y - 1 - worldPos.y;
            return new(x, y);
        }
        public static Vector2Int SnapToInt(Vector2 gridPos)
        {
            int x = Mathf.FloorToInt(gridPos.x);
            int y = Mathf.FloorToInt(gridPos.y);
            return new(x, y);
        }
        public static bool IsInGrid(GridSize gridSize, Vector2Int pos)
        {
            if (pos.x < 0)
            {
                return false;
            }
            if (pos.y < 0)
            {
                return false;
            }
            if (pos.x >= gridSize.X)
            {
                return false;
            }
            if (pos.y >= gridSize.Y)
            {
                return false;
            }
            return true;
        }
    }
}
