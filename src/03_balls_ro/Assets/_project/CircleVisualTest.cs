using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace VisualTests
{
    public sealed class CircleVisualTest : MonoBehaviour
    {
        [SerializeField] private BallSpawner _circleSpawner = null!;

        [FormerlySerializedAs("Scale")]
        public AnimatedValueConfig Radius = new()
        {
            ChangeRate = 1,
            Max = 5,
            Min = 1,
        };
        public AnimatedValueConfig X = new()
        {
            ChangeRate = 1,
            Max = 5,
            Min = -10,
        };
        public float ColorChangeRate = 5;

        [SerializeField] private Color[] _colors = {};

        private Transform[] _circleTransforms = null!;
        private readonly Ball[] _balls = { new(), new() };

        private void Start()
        {
            _circleTransforms = new Transform[_balls.Length];
            for (int i = 0; i < _balls.Length; i++)
            {
                var t = _circleSpawner.SpawnBall();
                _circleTransforms[i] = t;
            }

            _balls[1].Color = Color.white;
        }

        private void Update()
        {
            {
                var x = GetAnimatedValue(X);
                _balls[0].Position.x = x;

                var radius = GetAnimatedValue(Radius);
                _balls[0].Radius = radius;

                var colorIndex = GetAnimatedValue(new()
                {
                    ChangeRate = ColorChangeRate,
                    Min = 0,
                    Max = _colors.Length,
                });
                int baseline = (int) colorIndex;
                int firstColorIndex = (baseline) % _colors.Length;
                int secondColorIndex = (baseline + 1) % _colors.Length;
                float secondColorPortion = colorIndex - baseline;
                float firstColorPortion = 1 - secondColorPortion;
                Color a = _colors[firstColorIndex];
                Color b = _colors[secondColorIndex];
                Color c = a * firstColorPortion + b * secondColorPortion;
                _balls[0].Color = c;
            }

            {
                _balls[1].Position.x += 0.01f;
                _balls[1].Radius += 0.01f;
            }

            for (int i = 0; i < _balls.Length; i++)
            {
                BallEntity e = new(_balls[i], _circleTransforms[i]);
                BallHelper.Apply(e);
            }
        }


        private static float GetAnimatedValue(AnimatedValueConfig config)
        {
            var t01 = GetParameter01(config.ChangeRate);
            var ret = t01 * (config.Max - config.Min) + config.Min;
            return ret;
        }

        private static float GetParameter01(
            float changeRate)
        {
            var timeSinceStartInSeconds = Time.time;
            var sin = Math.Sin(changeRate * timeSinceStartInSeconds);
            var t02 = sin + 1;
            var t01 = t02 / 2;
            return (float) t01;
        }
    }

    [Serializable]
    public struct AnimatedValueConfig
    {
        public float ChangeRate;
        public float Min;
        public float Max;
    }
}