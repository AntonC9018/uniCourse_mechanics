using UnityEngine;

namespace VisualTests
{
    public sealed class Ball
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Radius;
        public Color Color;
    }

    public readonly record struct BallEntity(
        Ball Ball,
        Transform TransformInWorld);

    public static class BallHelper
    {
        public static void Apply(BallEntity b)
        {
            Vector3 pos = b.Ball.Position;
            pos.z = b.TransformInWorld.position.z;
            b.TransformInWorld.position = pos;

            var scale = b.Ball.Radius;
            var scaleVector = new Vector3(scale, scale, scale);
            b.TransformInWorld.localScale = scaleVector;

            var renderer_ = b.TransformInWorld.gameObject.GetComponent<SpriteRenderer>();
            renderer_.color = b.Ball.Color;
        }

        public static void ApplyMovement(Ball ball, float deltaTime)
        {
            var movementVec = ball.Velocity * deltaTime;
            ball.Position += movementVec;
        }
    }

}