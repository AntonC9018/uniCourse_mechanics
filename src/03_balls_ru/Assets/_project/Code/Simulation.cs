using UnityEngine;

namespace Core
{
    public sealed class Simulation : MonoBehaviour
    {
        [SerializeField] private BallSpawner _spawner = null!;
        [SerializeField] private Camera _camera = null!;

        [SerializeField] private BallData[] _ballData =
        {
            new()
            {
                Radius = 2.0f,
                Velocity = new(x: 2.0f, y: 0),
            },
            new()
            {
                Radius = 1.0f,
                Velocity = new(x: -2.0f, y: 0),
            },
        };
        private Transform[] _ballTransforms = null!;
        private Wall[]? _walls;

        public void Start()
        {
            _ballTransforms = new Transform[_ballData.Length];
            for (int i = 0; i < _ballData.Length; i++)
            {
                _ballTransforms[i] = _spawner.SpawnBall();
            }
        }

        public void Update()
        {
            var walls = WallHelper.GetWalls(_camera, ref _walls);

            for (int i = 0; i < _ballData.Length; i++)
            {
                var b = _ballData[i];

                UpdatePosition();
                WallHelper.HandleCollisionWithWalls(b, walls);
                continue;

                void UpdatePosition()
                {
                    var dt = Time.deltaTime;
                    var dp = dt * b.Velocity;
                    b.Position += dp;
                }
            }

            for (int i = 0; i < _ballData.Length; i++)
            {
                var b = _ballData[i];
                var t = _ballTransforms[i];
                BallHelper.Apply(b, t);
            }
        }
    }
}