using NUnit.Framework;
using UnityEngine;

namespace Core.Tests
{
    public sealed class GridTests
    {
        [Test]
        public void WorldToGrid_BackAndForth_Works()
        {
            var grid = new GridSize(5, 5);
            var gridPosition = GridHelper.WorldToGridPosition(grid, new(4, 3));
            var worldPosition = GridHelper.GridToWorldPosition(grid, gridPosition);
            Assert.AreEqual(new Vector2(4, 3), worldPosition);
        }
    }
}