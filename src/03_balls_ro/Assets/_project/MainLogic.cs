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

        [SerializeField] private Ball _ball = new()
        {
            Color = Color.white,
            Radius = 1,
            Velocity = new Vector2(1.5f, 0),
        };

        private Transform _ballTransform = null!;

        private void Start()
        {
            _ballTransform = _spawner.SpawnBall();
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
            Vector2 velocityTowardScene = speedTowardWall * -wallNormalTowardScene;
            ball.Velocity -= 2 * velocityTowardScene;
        }

        private void Update()
        {
            BallHelper.ApplyMovement(_ball, Time.deltaTime);

            var height = _camera.orthographicSize * 2;
            var width = _camera.aspect * height;
            (Vector2 Point, Vector2 NormalTowardScene)[] walls =
            {
                (Point: new(-width / 2, height / 2), NormalTowardScene: new(1, 0)),
                (Point: new(-width / 2, height / 2), NormalTowardScene: new(0, -1)),
                (Point: new(width / 2, -height / 2), NormalTowardScene: new(-1, 0)),
                (Point: new(width / 2, -height / 2), NormalTowardScene: new(0, 1)),
                (Point: new(0, height / 2), NormalTowardScene: new Vector2(-1, -1).normalized),
            };

            foreach (var w in walls)
            {
                if (IsBallCollidingWithWall(_ball, w.Point, w.NormalTowardScene))
                {
                    ResolveCollisionBallWall(_ball, w.NormalTowardScene);
                }
            }

            BallHelper.Apply(new(_ball, _ballTransform));
        }
    }
}