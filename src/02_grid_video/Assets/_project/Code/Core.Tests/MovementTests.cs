using Core;
using NUnit.Framework;
using UnityEngine;

public sealed class MovementTest
{
    [Test]
    public void GetDiagonalMoves()
    {
        var gridGo = new GameObject();
        var grid = gridGo.AddComponent<Core.Grid>();
        grid._size = new(4, 4);
        var playerPos = new Vector2Int(1, 1);

        var results = Moves.GetValidMovesDiagonal(grid, playerPos);
        Assert.That(results, Is.EqualTo(new Vector2Int[]
        {
            new(2, 2),
            new(3, 3),
            new(2, 0),
            new(0, 0),
            new(0, 2),
        }));
    }
}