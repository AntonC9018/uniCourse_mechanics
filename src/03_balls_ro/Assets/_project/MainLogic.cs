using System;
using UnityEngine;

namespace VisualTests
{
    public sealed class MainLogic : MonoBehaviour
    {
        [SerializeField] private BallSpawner _spawner = null!;
        [SerializeField] private Camera _camera = null!;

        private readonly Ball _ball = new()
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

        private void Update()
        {
            BallHelper.ApplyMovement(_ball, Time.deltaTime);

            // 1. Detectarea coliziunei cu peretele drept
            float ballRight = _ball.Position.x + _ball.Radius;
            float rightWallX;
            {
                var halfHeightInWorld = _camera.orthographicSize;
                float aspectRatio = _camera.aspect;

                // var w = pixelHeight / (halfHeightInWorld * 2);
                // rightWallX = pixelWidth / (w * 2);

                rightWallX = aspectRatio * halfHeightInWorld;
            }
            if (ballRight > rightWallX)
            {
                Debug.Log("Went over right wall");
            }


            BallHelper.Apply(new(_ball, _ballTransform));
        }
    }
}