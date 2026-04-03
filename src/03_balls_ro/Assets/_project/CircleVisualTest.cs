using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace VisualTests
{
    public sealed class Ball
    {
        public Vector2 Position;
        public float Radius;
        public Color Color;
    }

    public sealed class CircleVisualTest : MonoBehaviour
    {
        [SerializeField] private Transform _circlePrefab = null!;

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
                var t = Instantiate(_circlePrefab);
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
                Apply(_balls[i], _circleTransforms[i]);
            }
        }

        private static void Apply(Ball ball, Transform transform)
        {
            Vector3 pos = ball.Position;
            pos.z = transform.position.z;
            transform.position = pos;

            var scale = ball.Radius;
            var scaleVector = new Vector3(scale, scale, scale);
            transform.localScale = scaleVector;

            var renderer_ = transform.gameObject.GetComponent<SpriteRenderer>();
            renderer_.color = ball.Color;
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