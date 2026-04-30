using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Core
{
    public sealed class Simulation : MonoBehaviour
    {
        [SerializeField] private BallSpawner _spawner = null!;
        [SerializeField] private Camera _camera = null!;

        private BallData[] _ballData = null!;
        private Transform[] _ballTransforms = null!;
        private Wall[]? _walls;

        [SerializeField]
        [MinValue(1)]
        private int _ballCount = 10;

        [SerializeField]
        private Color[] _baseColors =
        {
            Color.white,
            Color.red,
            Color.green,
            Color.darkGreen,
            Color.blue,
            Color.lightBlue,
            Color.purple,
            Color.magenta,
            Color.brown,
            Color.orange,
        };

        private const float MinAllowedMass = 0.0001f;
        [SerializeField]
        [MinValue(MinAllowedMass)]
        private float MinMass;

        [SerializeField]
        [MinValue(MinAllowedMass)]
        private float MaxMass;

        public void Start()
        {
            _ballData = new BallData[_ballCount];

            if (MaxMass < MinMass)
            {
                var a = MaxMass;
                MaxMass = MinMass;
                MinMass = a;
            }

            for (int i = 0; i < _ballCount; i++)
            {
                var b = new BallData();
                _ballData[i] = b;

                // mass/radius
                float radius;
                {
                    float minMass = MinMass;
                    float maxMass = MaxMass;
                    float mass = UnityEngine.Random.Range(minMass, maxMass);
                    radius = mass;
                    b.Mass = mass;
                    b.Radius = radius;
                }

                // position = x,y
                {
                    float sceneMaxX = _camera.orthographicSize * _camera.aspect;
                    float maxX = sceneMaxX - radius;
                    float sceneMinX = -sceneMaxX;
                    float minX = sceneMinX + radius;

                    float sceneMaxY = _camera.orthographicSize;
                    float maxY = sceneMaxY - radius;
                    float sceneMinY = -sceneMaxY;
                    float minY = sceneMinY + radius;

                    float xPos = UnityEngine.Random.Range(minX, maxX);
                    float yPos = UnityEngine.Random.Range(minY, maxY);
                    b.Position = new Vector2(xPos, yPos);
                }

                // velocity = dir,s
                {
                    float maxSpeed = 4.0f;
                    b.Velocity = UnityEngine.Random.insideUnitCircle * maxSpeed;
                }

                // color
                {
                    int colorIndex1 = UnityEngine.Random.Range(0, _baseColors.Length);
                    Color color1 = _baseColors[colorIndex1];
                    int colorIndex2 = UnityEngine.Random.Range(0, _baseColors.Length);
                    Color color2 = _baseColors[colorIndex2];
                    float p = UnityEngine.Random.Range(0.0f, 1.0f);
                    b.Color = Color.LerpUnclamped(color1, color2, p);
                }
            }

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

            float GetMaxRadius()
            {
                float maxRadius = _ballData[0].Radius;
                for (int i = 1; i < _ballData.Length; i++)
                {
                    var r = _ballData[i].Radius;
                    if (r > maxRadius)
                    {
                        maxRadius = r;
                    }
                }
                return maxRadius;
            }

            {
                var cellSize = GetMaxRadius() * 2;
                var sy = _camera.orthographicSize * 2;
                var sx = _camera.aspect * sy;
                var s = new Vector2(sx, sy);

                Vector2Int WorldToGrid(Vector2 w)
                {
                    var t = (w + s / 2);
                    t.x /= cellSize;
                    t.y /= cellSize;
                    var ret = new Vector2Int(
                        x: Mathf.FloorToInt(t.x),
                        y: Mathf.FloorToInt(t.y));
                    return ret;
                }

                Dictionary<Vector2Int, List<int>> uniformGrid = new(_ballData.Length);

                for (int i = 0; i < _ballData.Length; i++)
                {
                    var b = _ballData[i];
                    var positionInWorldSpace = b.Position;
                    var gridPos = WorldToGrid(positionInWorldSpace);
                    if (!uniformGrid.TryGetValue(gridPos, out var list))
                    {
                        list = new();
                        uniformGrid.Add(gridPos, list);
                    }

                    list.Add(i);
                }

                ReadOnlySpan<Vector2Int> neighborOffsets = stackalloc Vector2Int[4]
                {
                    new(x: 1, y: 1),
                    new(x: 1, y: 0),
                    new(x: 1, y: -1),
                    new(x: 0, y: -1),
                };

                for (int i1 = 0; i1 < _ballData.Length; i1++)
                {
                    var ball1 = _ballData[i1];
                    var positionInWorldSpace = ball1.Position;
                    var gridPos = WorldToGrid(positionInWorldSpace);

                    // Своя клетка + id
                    Self();
                    void Self()
                    {
                        if (!uniformGrid.TryGetValue(gridPos, out var list))
                        {
                            return;
                        }
                        foreach (var i2 in list)
                        {
                            if (i1 >= i2)
                            {
                                continue;
                            }

                            ProcessPotentialCollision(i1, i2);
                        }
                    }

                    // Соседние клетки
                    {
                        foreach (var offset in neighborOffsets)
                        {
                            var gridPos2 = gridPos + offset;
                            if (!uniformGrid.TryGetValue(gridPos2, out var list))
                            {
                                continue;
                            }
                            foreach (var i2 in list)
                            {
                                ProcessPotentialCollision(i1, i2);
                            }
                        }
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

        private void ProcessPotentialCollision(int i1, int i2)
        {
            var b1 = _ballData[i1];
            var b2 = _ballData[i2];
            ref var p1 = ref b1.Position;
            ref var p2 = ref b2.Position;
            var d = p2 - p1;
            var dlen = d.magnitude;
            if (dlen == 0)
            {
                d = new(1, 0);
                dlen = 1;
            }

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
                    {
                        var m1 = b1.Mass;
                        var m2 = b2.Mass;
                        var ms = m1 + m2;
                        var centerV = (m1 * a1 + m2 * a2n) / ms;
                        var v1New = -a1 + 2 * centerV;
                        var v2New = -a2n + 2 * centerV;
                        v1 = v1 + (-a1 + v1New) * n;
                        v2 = v2 + (-a2n + v2New) * n;
                    }

                    {
                        // Move out of collision
                        var x = rsum - dlen;
                        var c1 = r1 / rsum;
                        var c2 = r2 / rsum;
                        p1 = p1 - c1 * x * n;
                        p2 = p2 + c2 * x * n;
                    }
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
    }
}