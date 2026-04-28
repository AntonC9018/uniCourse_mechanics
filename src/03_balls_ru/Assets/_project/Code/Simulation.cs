using System.Drawing;
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

        private static bool IsBallCollidingWithWall(
            BallData ball,
            Vector2 wallPoint,
            Vector2 normalTowardScene)
        {
            var extremity = ball.Position - ball.Radius * normalTowardScene;
            var d = wallPoint - extremity;
            var dot = Vector2.Dot(d, normalTowardScene);
            if (dot >= 0)
            {
                return true;
            }
            return false;
        }

        private static void HandleCollisionWithWall(
            BallData ball,
            Vector2 normalTowardScene)
        {
            var v = ball.Velocity;
            var dot = Vector2.Dot(v, normalTowardScene);
            if (dot < 0)
            {
                var vn = dot * normalTowardScene;
                var vNew = v - 2 * vn;
                ball.Velocity = vNew;
            }
        }

        public void Update()
        {
            var dt = Time.deltaTime;
            var dp = dt * _ballData.Velocity;
            _ballData.Position += dp;


            var halfwidth = _camera.aspect * _camera.orthographicSize;
            var halfheight = _camera.orthographicSize;

            (Vector2 Point, Vector2 NormalTowardScene)[] walls =
            {
                (new(halfwidth, 0), new(-1, 0)),
                (new(-halfwidth, 0), new(1, 0)),
                (new(0, halfheight), new(0, -1)),
                (new(0, -halfheight), new(0, 1)),
                (new(halfwidth / 2, halfheight), new Vector2(-1, -1).normalized),
            };
            foreach (var w in walls)
            {
                if (IsBallCollidingWithWall(_ballData, w.Point, w.NormalTowardScene))
                {
                    HandleCollisionWithWall(_ballData, w.NormalTowardScene);
                }
            }

            BallHelper.Apply(_ballData, _ballTransform);
        }
    }
}