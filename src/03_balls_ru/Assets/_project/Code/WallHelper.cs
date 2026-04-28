using System;
using UnityEngine;

namespace Core
{
    public readonly record struct Wall(
        Vector2 Point,
        Vector2 NormalTowardScene);

    public static class WallHelper
    {
        public static void HandleCollisionWithWalls(
            BallData b,
            Wall[] walls)
        {
            foreach (var w in walls)
            {
                if (IsBallCollidingWithWall(b, w.Point, w.NormalTowardScene))
                {
                    HandleCollisionWithWall(b, w.NormalTowardScene);
                }
            }
        }

        public static Wall[] GetWalls(
            Camera camera,
            ref Wall[]? walls)
        {
            var halfwidth = camera.aspect * camera.orthographicSize;
            var halfheight = camera.orthographicSize;
            Array.Resize(ref walls, 4);

            walls[0] = new(new(halfwidth, 0), new(-1, 0));
            walls[1] = new(new(-halfwidth, 0), new(1, 0));
            walls[2] = new(new(0, halfheight), new(0, -1));
            walls[3] = new(new(0, -halfheight), new(0, 1));

            return walls;
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
    }
}