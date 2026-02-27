using UnityEngine;

namespace Core
{
    public sealed class Grid : MonoBehaviour
    {
        [SerializeField]
        [Min(1)]
        private Vector2Int _size = new(10, 10);

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

        public Vector2Int SnapToCellOrigin(Vector2 gridPos)
        {
            Vector2Int ret = default;
            ret.x = Mathf.FloorToInt(gridPos.x);
            ret.y = Mathf.FloorToInt(gridPos.y);
            return ret;
        }

        public Transform? FindCellAt(Vector2Int gridPos)
        {
            foreach (Transform cell in transform)
            {
                Vector2 fpos = cell.position;
                Vector2Int pos = default;
                pos.x = (int) fpos.x;
                pos.y = (int) fpos.y;

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
