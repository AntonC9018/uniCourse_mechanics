using System;
using UnityEngine;

namespace Core
{
    [Serializable]
    public sealed class BallData
    {
        public float Radius = 1.0f;
        public Vector2 Position;
        public Vector2 Velocity;
    }

    public static class BallHelper
    {
        public static void Apply(BallData ball, Transform ballTransform)
        {
            var s = ball.Radius * 2;
            ballTransform.localScale = new(s, s, s);

            var position = ballTransform.localPosition;
            position.x = ball.Position.x;
            position.y = ball.Position.y;
            ballTransform.localPosition = position;
        }
    }
}