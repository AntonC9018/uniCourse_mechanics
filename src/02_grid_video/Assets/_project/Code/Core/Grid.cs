using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

[assembly: InternalsVisibleTo("Core.Tests")]

namespace Core
{
    public sealed class Grid : MonoBehaviour
    {
        [SerializeField]
        [Min(1)]
        internal Vector2Int _size = new(10, 10);

        public Vector2Int Size => _size;

        public Vector2 GridToWorld(Vector2 gridPos)
        {
            Vector2 result;
            result.x = gridPos.x;
            result.y = -gridPos.y;
            return result;
        }

        public Vector2 WorldToGrid(Vector2 gridPos)
        {
            Vector2 result;
            result.x = gridPos.x;
            result.y = -gridPos.y;
            return result;
        }

        public Vector2Int WorldToGridOfCellOrigin(Vector3 posWorldSpace)
        {
            var gridSpace = WorldToGrid(posWorldSpace);
            var ret = MakeSureCellOrigin(gridSpace);
            return ret;
        }

        public Vector2Int SnapToCellOrigin(Vector2 gridPos)
        {
            Vector2Int ret = default;
            ret.x = Mathf.FloorToInt(gridPos.x);
            ret.y = Mathf.FloorToInt(gridPos.y);
            return ret;
        }

        public Vector2Int MakeSureCellOrigin(Vector2 gridPos)
        {
            Assert.AreEqual(gridPos.x, Mathf.Floor(gridPos.x));
            Assert.AreEqual(gridPos.y, Mathf.Floor(gridPos.y));
            var ret = SnapToCellOrigin(gridPos);
            return ret;
        }

        public bool IsInGrid(Vector2Int gridPos)
        {
            if (gridPos.x < 0)
            {
                return false;
            }
            if (gridPos.x >= _size.x)
            {
                return false;
            }
            if (gridPos.y < 0)
            {
                return false;
            }
            if (gridPos.y >= _size.y)
            {
                return false;
            }
            return true;
        }

        public Transform? FindCellAt(Vector2Int gridPos)
        {
            foreach (Transform cell in transform)
            {
                Vector2 fposWorldSpace = cell.position;
                Vector2 fposGridSpace = WorldToGrid(fposWorldSpace);
                var pos = MakeSureCellOrigin(fposGridSpace);
                if (pos == gridPos)
                {
                    return cell;
                }
            }
            return null;
        }
    }

    public static class CellHelper
    {
        public static SpriteRenderer GetSpriteRenderer(GameObject cell)
        {
            var ret = cell.GetComponentInChildren<SpriteRenderer>();
            return ret;
        }
    }
}
