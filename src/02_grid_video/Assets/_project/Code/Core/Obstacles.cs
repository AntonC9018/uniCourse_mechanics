using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public sealed class Obstacles : MonoBehaviour
    {
        [SerializeField] private Grid _grid = null!;
        [SerializeField] private GameObject _obstaclePrefab = null!;
        [SerializeField] private Player _player = null!;

        private readonly List<Obstacle> _obstacles = new();

        private void Start()
        {
            const int obstacleCount = 10;
            var parent = transform;
            for (int i = 0; i < obstacleCount; i++)
            {
                var gridPos = GetRandomPositionForObstacle();
                var worldPos = _grid.GridToWorld(gridPos);
                var cell = Instantiate(_obstaclePrefab);
                cell.name = $"Obstacle at: x={gridPos.x}, y={gridPos.y}";
                var t = cell.transform;
                t.SetParent(parent);
                t.localPosition = worldPos;

                _obstacles.Add(new()
                {
                    GridPosition = gridPos,
                });
            }
        }

        private Vector2Int GetRandomPositionForObstacle()
        {
            Vector2Int gridPos;
            while (true)
            {
                var gridSize = _grid.Size;
                var x = Random.Range(0, gridSize.x);
                var y = Random.Range(0, gridSize.y);
                gridPos = new Vector2Int(x, y);

                if (_player.GridPosition == gridPos)
                {
                    continue;
                }

                bool ExistsObstacleAtPos()
                {
                    foreach (var o in _obstacles)
                    {
                        if (o.GridPosition == gridPos)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                if (ExistsObstacleAtPos())
                {
                    continue;
                }
                break;
            }
            return gridPos;
        }

        public bool HasObstacleAt(Vector2Int pos)
        {
            foreach (var ob in _obstacles)
            {
                if (ob.GridPosition == pos)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public struct Obstacle
    {
        // public Transform Transform;
        public Vector2Int GridPosition;
    }
}
