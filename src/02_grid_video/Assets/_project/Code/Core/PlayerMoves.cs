using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Core
{
    public enum MoveSet
    {
        Orthogonal,
        Diagonal,
    }

    public readonly record struct MoveDerivationData(
        MoveSet MoveSet,
        int MaxDistance);


    public readonly record struct ValidMoves(
        List<Vector2Int> Positions);

    public sealed class PlayerMovesService : MonoBehaviour
    {
        [SerializeField] private Grid _grid = null!;
        [SerializeField] private Obstacles _obstacles = null!;

        public bool CheckValidMove(
            ValidMoves result,
            MoveDerivationData data,
            Vector2Int newPos)
        {
            _ = data;
            if (result.Positions.Contains(newPos))
            {
                return true;
            }
            return false;
        }

        public ValidMoves GetValidMoves(
            ValidMoves result,
            Vector2Int playerPos,
            MoveDerivationData data)
        {
            if (result.Positions == null)
            {
                result = new(new());
            }

            result.Positions.Clear();
            switch (data.MoveSet)
            {
                case MoveSet.Orthogonal:
                {
                    Logic(new(1, 0));
                    break;
                }
                case MoveSet.Diagonal:
                {
                    Logic(new(1, 1));
                    break;
                }
                default:
                {
                    Assert.IsTrue(false, "Fail");
                    break;
                }
            }

            void Logic(Vector2Int baseDirection)
            {
                GetValidMovesForBaseDirection(
                    result.Positions,
                    data.MaxDistance,
                    playerPos: playerPos,
                    baseDir: baseDirection);
            }

            return result;
        }

        private readonly List<Vector2Int> _cache = new();

        private void GetValidMovesForBaseDirection(
            List<Vector2Int> result,
            int maxDistance,
            Vector2Int playerPos,
            Vector2Int baseDir)
        {
            var dirs = Moves.RotatedInAllDirections(baseDir);
            foreach (var dir in dirs)
            {
                var positionsInThisDir = _cache;
                positionsInThisDir.Clear();

                Moves.GetCellsInDirection(new(
                    positionsInThisDir,
                    _grid,
                    playerPos,
                    dir));

                int currentDist = 0;

                foreach (var pos in positionsInThisDir)
                {
                    currentDist += 1;
                    if (currentDist > maxDistance)
                    {
                        break;
                    }
                    if (_obstacles.HasObstacleAt(pos))
                    {
                        break;
                    }

                    result.Add(pos);
                }
            }
        }
    }

    public static class Moves
    {
        public static void GetAllDiagonalPosition(
            List<Vector2Int> result,
            Grid grid,
            Vector2Int playerPos)
        {
            Vector2Int currentDir = new(1, 1);
            var p = new GetCellInDirectionParams(
                result,
                grid,
                StartPos: playerPos,
                Dir: currentDir);
            GetCellsInDirectionRotated(p);
        }


        public readonly struct RotatedInAllDirectionsEnumerable
        {
            private readonly Vector2Int _dir;

            public RotatedInAllDirectionsEnumerable(Vector2Int dir)
            {
                Assert.AreNotEqual(dir, Vector2Int.zero);
                _dir = dir;
            }

            public Enumerator GetEnumerator() => new(_dir);

            public struct Enumerator
            {
                private Vector2Int _dir;
                private int _i;

                public Enumerator(Vector2Int dir)
                {
                    Assert.AreNotEqual(dir, Vector2Int.zero);
                    _dir = dir;
                    _i = -1;
                }

                public Vector2Int Current => _dir;
                public bool MoveNext()
                {
                    _i++;
                    if (_i == 0)
                    {
                        return true;
                    }
                    if (_i == 4)
                    {
                        return false;
                    }
                    _dir = new(_dir.y, -_dir.x);
                    return true;
                }
            }
        }
        public static RotatedInAllDirectionsEnumerable RotatedInAllDirections(Vector2Int dir) => new(dir);

        public static void GetCellsInDirectionRotated(GetCellInDirectionParams p)
        {
            foreach (var dir in RotatedInAllDirections(p.Dir))
            {
                GetCellsInDirection(p with
                {
                    Dir = dir,
                });
            }
        }

        public static void GetCellsInDirection(GetCellInDirectionParams p)
        {
            Assert.AreNotEqual(p.Dir, Vector2Int.zero);
            Vector2Int currentPos = p.StartPos;
            while (true)
            {
                currentPos += p.Dir;
                if (!p.Grid.IsInGrid(currentPos))
                {
                    break;
                }

                p.Result.Add(currentPos);
            }
        }
    }

    public readonly record struct GetCellInDirectionParams(
        List<Vector2Int> Result,
        Grid Grid,
        Vector2Int StartPos,
        Vector2Int Dir);
}
