using System;
using UnityEngine;

namespace Core
{
    public sealed class VisualTest : MonoBehaviour
    {
        [SerializeField] private BallSpawner _spawner = null!;

        private Transform _ballTransform = null!;
        [SerializeField] private BallData _ballData = new();

        public AnimationParameters RadiusParams = new()
        {
            Min = 1.0f,
            Max = 3.0f,
            ChangeRate = 1.0f,
            Seed = 0.0f,
        };
        public AnimationParameters XParams = new()
        {
            Min = 0.0f,
            Max = 10.0f,
            ChangeRate = 1.0f,
            Seed = 15.5f,
        };

        private void Start()
        {
            _ballTransform = _spawner.SpawnBall();
        }

        private void Update()
        {
            _ballData.Radius = RadiusParams.GetAnimatedValue();
            _ballData.Position.x = XParams.GetAnimatedValue();

            BallHelper.Apply(_ballData, _ballTransform);
        }
    }

    [Serializable]
    public struct AnimationParameters
    {
        public float Min;
        public float Max;
        public float ChangeRate;
        public float Seed;

        public readonly float GetAnimatedValue()
        {
            var ret = GetSinValue(
                time: Time.time * this.ChangeRate + this.Seed,
                min: this.Min,
                max: this.Max);
            return ret;
        }

        private static float GetSinValue(float time, float min, float max)
        {
            var s = Mathf.Sin(time);
            var s02 = s + 1;
            var s01 = s02 / 2;
            var range = max - min;
            var ret = s01 * range + min;
            return ret;
        }
    }
}