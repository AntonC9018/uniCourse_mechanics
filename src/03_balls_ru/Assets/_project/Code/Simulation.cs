using UnityEngine;

namespace Core
{
    public sealed class Simulation : MonoBehaviour
    {
        [SerializeField] private BallSpawner _spawner = null!;
        [SerializeField] private Camera _camera = null!;

        [SerializeField] private BallData[] _ballData =
        {
            new()
            {
                Radius = 2.0f,
                Velocity = new(x: 2.0f, y: 0),
            },
            new()
            {
                Radius = 1.0f,
                Velocity = new(x: -2.0f, y: 0),
            },
        };
        private Transform[] _ballTransforms = null!;
        private Wall[]? _walls;

        public void Start()
        {
            _ballTransforms = new Transform[_ballData.Length];
            for (int i = 0; i < _ballData.Length; i++)
            {
                _ballTransforms[i] = _spawner.SpawnBall();
            }
        }

        public void Update()
        {
            var walls = WallHelper.GetWalls(_camera, ref _walls);

            for (int i = 0; i < _ballData.Length; i++)
            {
                var b = _ballData[i];

                UpdatePosition();
                WallHelper.HandleCollisionWithWalls(b, walls);
                continue;

                void UpdatePosition()
                {
                    var dt = Time.deltaTime;
                    var dp = dt * b.Velocity;
                    b.Position += dp;
                }
            }

            {
                var b1 = _ballData[0];
                var b2 = _ballData[1];
                ref var p1 = ref b1.Position;
                ref var p2 = ref b2.Position;
                var d = p2 - p1;
                var dlen = d.magnitude;

                var r1 = b1.Radius;
                var r2 = b2.Radius;
                var rsum = r1 + r2;
                if (dlen <= rsum)
                {
                    var n = d / dlen;

                    ref var v1 = ref b1.Velocity;
                    ref var v2 = ref b2.Velocity;

                    var a1 = Vector2.Dot(v1, n);
                    var a2n = Vector2.Dot(v2, n);

                    if (ShouldProcessCollision())
                    {
                        var v1n = a1 * n;
                        var v2n = a2n * n;

                        v1 = v1 - v1n + v2n;
                        v2 = v2 - v2n + v1n;

                        // Move out of collision
                        var x = rsum - dlen;
                        var c1 = r1 / rsum;
                        var c2 = r2 / rsum;
                        p1 = p1 - c1 * x * n;
                        p2 = p2 + c2 * x * n;
                    }

                    bool ShouldProcessCollision()
                    {
                        return true;
                        #if false
                        var a2 = -a2n;
                        if (a1 > 0 && a2 > 0)
                        {
                            return true;
                        }
                        if (a1 < 0 && a2 < 0)
                        {
                            return false;
                        }
                        if (a1 > 0)
                        {
                            return a1 > -a2;
                        }
                        // if (a2 > 0)
                        {
                            return a2 > -a1;
                        }
                        #endif
                    }
                }
            }

            for (int i = 0; i < _ballData.Length; i++)
            {
                var b = _ballData[i];
                var t = _ballTransforms[i];
                BallHelper.Apply(b, t);
            }
        }
    }
}