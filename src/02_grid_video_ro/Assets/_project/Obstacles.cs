using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [DefaultExecutionOrder(Player.ExecutionOrder - 1)]
    public sealed class Obstacles : MonoBehaviour
    {
        [SerializeField] private Grid _grid = null!;
        [SerializeField] private Transform _player = null!;
        [SerializeField] private GameObject _obstaclePrefab = null!;

        private readonly List<Vector2Int> _obstacles = new();
        private LookupDataStructure _lookup;

        // O(1) - N numarul de obstacole
        public bool HasObstacleAt(Vector2Int v)
        {
            return _lookup.Check(v);
        }

        private void Start()
        {
            var parent = transform;

            const int obstacleCount = 10;
            for (int i = 0; i < obstacleCount; i++)
            {
                var pos = GenerateRandomPositionForObstacle();
                _obstacles.Add(pos);

                Vector3 worldPos3 = _grid.GridToWorld(pos);
                worldPos3.z = 0;

                var obstacle = GameObject.Instantiate(
                    _obstaclePrefab,
                    position: worldPos3,
                    rotation: Quaternion.identity,
                    parameters: new()
                    {
                        worldSpace = false,
                        parent = parent,
                    });
                _ = obstacle;
            }

            _lookup = LookupDataStructure.Create(_grid, _obstacles);
        }

        private Vector2Int GenerateRandomPositionForObstacle()
        {
            var s = _grid.Size;

            while (true)
            {
                int x = Random.Range(0, s.x);
                int y = Random.Range(0, s.y);
                var pos = new Vector2Int(x, y);
                if (_obstacles.Contains(pos))
                {
                    continue;
                }

                var playerPos = _player.position;
                var playerPosInGridSpace = _grid.WorldToGrid(playerPos);
                if (pos == playerPosInGridSpace)
                {
                    continue;
                }

                return pos;
            }
        }
    }

    public readonly struct LookupDataStructure
    {
        private readonly bool[][] _uniformGrid;

        private LookupDataStructure(bool[][] uniformGrid)
        {
            _uniformGrid = uniformGrid;
        }

        public static LookupDataStructure Create(
            Grid grid,
            List<Vector2Int> occupied)
        {
            var uniformGrid = new bool[grid.Size.y][];
            for (int rowIndex = 0; rowIndex < uniformGrid.Length; rowIndex++)
            {
                uniformGrid[rowIndex] = new bool[grid.Size.x];
            }

            foreach (var o in occupied)
            {
                uniformGrid[o.y][o.x] = true;
            }
            return new(uniformGrid);
        }

        public bool Check(Vector2Int v)
        {
            bool exists = _uniformGrid[v.y][v.x];
            return exists;
        }
    }
}