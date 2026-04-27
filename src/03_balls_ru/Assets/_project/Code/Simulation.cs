using UnityEngine;

namespace Core
{
    public sealed class Simulation : MonoBehaviour
    {
        [SerializeField] private BallSpawner _spawner = null!;
        [SerializeField] private Camera _camera = null!;

        [SerializeField] private BallData _ballData = new()
        {
            Radius = 2.0f,
            Velocity = new(x: 2.0f, y: 0),
        };
        private Transform _ballTransform = null!;

        public void Start()
        {
            _ballTransform = _spawner.SpawnBall();
        }

        public void Update()
        {
            var dt = Time.deltaTime;
            var dp = dt * _ballData.Velocity;
            _ballData.Position += dp;

            {
                var extremity = _ballData.Position;
                extremity.x += _ballData.Radius;

                float rightWallX = _camera.aspect * _camera.orthographicSize;
                if (extremity.x > rightWallX)
                {
                    ref var vx = ref _ballData.Velocity.x;
                    if (vx > 0)
                    {
                        vx = -vx;
                    }
                }
            }
            {
                var extremity = _ballData.Position;
                extremity.x -= _ballData.Radius;

                float leftWallX = -_camera.aspect * _camera.orthographicSize;
                if (extremity.x < leftWallX)
                {
                    ref var vx = ref _ballData.Velocity.x;
                    if (vx < 0)
                    {
                        vx = -vx;
                    }
                }
            }
            {
                var extremity = _ballData.Position;
                extremity.y += _ballData.Radius;

                float topWallY = _camera.orthographicSize;
                if (extremity.y > topWallY)
                {
                    ref var vy = ref _ballData.Velocity.y;
                    if (vy > 0)
                    {
                        vy = -vy;
                    }
                }
            }
            {
                var extremity = _ballData.Position;
                extremity.y -= _ballData.Radius;

                float topWallY = -_camera.orthographicSize;
                if (extremity.y < topWallY)
                {
                    ref var vy = ref _ballData.Velocity.y;
                    if (vy < 0)
                    {
                        vy = -vy;
                    }
                }
            }

            BallHelper.Apply(_ballData, _ballTransform);
        }
    }
}