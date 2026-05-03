using System;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

namespace VisualTests
{
    public sealed class MainLogic : MonoBehaviour
    {
        [SerializeField] private BallSpawner _spawner = null!;
        [SerializeField] private Camera _camera = null!;

        [SerializeField] private Ball[] _balls =
        {
            new()
            {
                Color = Color.white,
                Radius = 1,
                Velocity = new Vector2(1.5f, 0),
            },
            new()
            {
                Color = Color.red,
                Radius = 2,
                Velocity = new Vector2(-1.5f, 0),
                Position = new Vector2(2, 0),
            },
        };

        private Transform[] _ballTransforms = null!;

        private void Start()
        {
            _ballTransforms = new Transform[_balls.Length];
            for (int i = 0; i < _balls.Length; i++)
            {
                _ballTransforms[i] = _spawner.SpawnBall();
            }
        }

        private void Update()
        {
            for (int i = 0; i < _balls.Length; i++)
            {
                var ball = _balls[i];
                BallHelper.ApplyMovement(ball, Time.deltaTime);
            }

            HandleWallCollisions();

            {
                var b1 = _balls[0];
                var b2 = _balls[1];
                ref var p1 = ref b1.Position;
                ref var p2 = ref b2.Position;
                var d = p2 - p1;
                var dlen = d.magnitude;
                var r1 = b1.Radius;
                var r2 = b2.Radius;
                var rsum = r1 + r2;
                var overlap = rsum - dlen;
                if (overlap >= 0)
                {
                    ref var v1 = ref b1.Velocity;
                    ref var v2 = ref b2.Velocity;
                    var n = d / dlen;

                    var a1 = Vector2.Dot(v1, n);
                    var a2 = -Vector2.Dot(v2, n);
                    // if (a1 + a2 > 0)
                    {
                        var v1n = a1 * n;
                        var v2n = -a2 * n;
                        v1 = v1 - v1n + v2n;
                        v2 = v2 - v2n + v1n;
                    }

                    {
                        var portion1 = r1 / rsum;
                        var portion2 = 1 - portion1;
                        var d1 = portion1 * overlap * -n;
                        var d2 = portion2 * overlap * n;
                        p1 += d1;
                        p2 += d2;
                    }
                }
            }

            for (int i = 0; i < _balls.Length; i++)
            {
                var ball = _balls[i];
                BallHelper.Apply(new(ball, _ballTransforms[i]));
            }
        }

        private void HandleWallCollisions()
        {
            var height = _camera.orthographicSize * 2;
            var width = _camera.aspect * height;
            ReadOnlySpan<Wall> walls = stackalloc Wall[4]
            {
                new(Point: new(-width / 2, height / 2), NormalTowardScene: new(1, 0)),
                new(Point: new(-width / 2, height / 2), NormalTowardScene: new(0, -1)),
                new(Point: new(width / 2, -height / 2), NormalTowardScene: new(-1, 0)),
                new(Point: new(width / 2, -height / 2), NormalTowardScene: new(0, 1)),
            };

            for (int i = 0; i < _balls.Length; i++)
            {
                var ball = _balls[i];
                foreach (var w in walls)
                {
                    if (IsBallCollidingWithWall(ball, w.Point, w.NormalTowardScene))
                    {
                        ResolveCollisionBallWall(ball, w.NormalTowardScene);
                    }
                }
            }
        }

        private static bool IsBallCollidingWithWall(
            Ball ball,
            Vector2 wallPoint,
            Vector2 wallNormalTowardScene)
        {
            // 1. extremitatea
            Vector2 extremity = ball.Position - ball.Radius * wallNormalTowardScene;
            // 2. difference
            Vector2 diff = extremity - wallPoint;
            // 3. dot(difference, normal) < 0
            float dot = Vector2.Dot(diff, wallNormalTowardScene);
            if (dot <= 0)
            {
                return true;
            }
            return false;
        }

        private static void ResolveCollisionBallWall(
            Ball ball,
            Vector2 wallNormalTowardScene)
        {
            // 1. cata viteza merge spre perete
            // 2. eliminam aceasta bucata
            // 3. o scadem inca o data (pentru a o reflecta)

            float speedTowardWall = -Vector2.Dot(ball.Velocity, wallNormalTowardScene);
            if (speedTowardWall > 0)
            {
                Vector2 velocityTowardScene = speedTowardWall * -wallNormalTowardScene;
                ball.Velocity -= 2 * velocityTowardScene;
            }
        }

    }

    public readonly record struct Wall(
        Vector2 Point,
        Vector2 NormalTowardScene);
}