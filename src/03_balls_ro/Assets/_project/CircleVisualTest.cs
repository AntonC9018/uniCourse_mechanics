using System;
using UnityEngine;

namespace VisualTests
{
    public sealed class CircleVisualTest : MonoBehaviour
    {
        [SerializeField] private Transform _circleTransform = null!;

        public AnimatedValueConfig Scale = new()
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

        private void Update()
        {
            {
                var x = GetAnimatedValue(X);

                var currentPos = _circleTransform.position;
                currentPos.x = x;

                _circleTransform.position = currentPos;
            }

            {
                var scale = GetAnimatedValue(Scale);
                var scaleVector = new Vector3(scale, scale, scale);
                _circleTransform.localScale = scaleVector;
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