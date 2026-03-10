using UnityEngine.Assertions;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Core
{
    public sealed class Grid : MonoBehaviour
    {
        [SerializeField] private Vector2Int _size = new(5, 10);
        public Vector2Int Size => _size;

        public Vector2 GridToWorld(Vector2 gridPos)
        {
            Vector2 ret;
            ret.x = gridPos.x;
            ret.y = -gridPos.y;
            return ret;
        }

        public Vector2 WorldToGrid(Vector2 worldPos)
        {
            Vector2 ret;
            ret.x = worldPos.x;
            ret.y = -worldPos.y;
            return ret;
        }

        public Vector2Int SnapToCellOrigin(Vector2 pos)
        {
            Vector2Int ret = default;
            ret.x = Mathf.FloorToInt(pos.x);
            ret.y = Mathf.FloorToInt(pos.y);
            return ret;
        }

        public Vector2Int MakeSureCellOrigin(Vector2 gridPos)
        {
            var snapped = SnapToCellOrigin(gridPos);
            Vector2 f = snapped;
            Assert.AreEqual(f, gridPos);
            return snapped;
        }

        public bool IsInGrid(Vector2Int cellOriginPos)
        {
            if (cellOriginPos.x < 0)
            {
                return false;
            }
            if (cellOriginPos.x >= Size.x)
            {
                return false;
            }
            if (cellOriginPos.y < 0)
            {
                return false;
            }
            if (cellOriginPos.y >= Size.y)
            {
                return false;
            }
            return true;
        }

        public Transform? FindCellAt(Vector2Int cellOriginPos)
        {
            foreach (Transform cell in transform)
            {
                var cellWorldPos = cell.position;
                var cellGridPos = WorldToGrid(cellWorldPos);
                var gridPosInt = MakeSureCellOrigin(cellGridPos);

                if (cellOriginPos == gridPosInt)
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
            var renderer1 = cell.GetComponentInChildren<SpriteRenderer>();
            return renderer1;
        }
    }
}