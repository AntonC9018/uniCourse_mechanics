using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Core
{
    public sealed class MoveSelection : MonoBehaviour
    {
        [SerializeField] private Grid _grid = null!;
        public int MaxDistance = 3;
        public List<Vector2Int> SelectedValidMoves { get; } = new();

        public bool CheckValidMove(Vector2Int pos)
        {
            return SelectedValidMoves.Contains(pos);
        }

        public void ResetMoves(
            Vector2Int playerPos,
            MoveSet moveSet)
        {
            SelectedValidMoves.Clear();

            var orthoBaseDir = new Vector2Int(1, 0);
            var diagBaseDir = new Vector2Int(1, 1);
            switch (moveSet)
            {
                case MoveSet.Orthogonal:
                {
                    Logic(this, orthoBaseDir);
                    break;
                }
                case MoveSet.Diagonal:
                {
                    Logic(this, diagBaseDir);
                    break;
                }
                case MoveSet.Queen:
                {
                    Logic(this, orthoBaseDir);
                    Logic(this, diagBaseDir);
                    break;
                }
                default:
                {
                    Assert.IsTrue(false, $"Not handled move set {moveSet}");
                    return;
                }
            }

            void Logic(MoveSelection self, Vector2Int baseDir)
            {
                self.AddValidMovesForDir(
                    playerGridPos: playerPos,
                    baseDir: baseDir,
                    output: self.SelectedValidMoves);
            }
        }

        private void AddValidMovesForDir(
            Vector2Int playerGridPos,
            Vector2Int baseDir,
            List<Vector2Int> output)
        {
            var dir = baseDir;
            for (int i = 0; i < 4; i++)
            {
                var currentPos = playerGridPos;
                int currentDist = 0;
                while (true)
                {
                    if (currentDist >= MaxDistance)
                    {
                        break;
                    }
                    currentDist++;

                    currentPos += dir;
                    if (!_grid.IsInGrid(currentPos))
                    {
                        break;
                    }
                    output.Add(currentPos);
                }

                dir = new(x: -dir.y, y: dir.x);
            }
        }
    }
}