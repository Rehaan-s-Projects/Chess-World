using NUnit.Framework;
using UnityEngine;
using ChessWorld.Core;

namespace ChessWorld.Game.Tests
{
    public class BoardViewTests
    {
        [Test]
        public void WorldSquareRoundTrip()
        {
            var go = new GameObject("TestBoard");
            var bv = go.AddComponent<BoardView>();
            bv.SquareSize = 1f;
            for (int i = 0; i < 64; i++)
            {
                var sq = Square.FromIndex(i);
                var world = bv.WorldOf(sq);
                Assert.That(bv.TryWorldToSquare(world, out var back), Is.True);
                Assert.That(back, Is.EqualTo(sq));
            }
            Object.DestroyImmediate(go);
        }

        [Test]
        public void TryWorldToSquare_OutOfBounds_ReturnsFalse()
        {
            var go = new GameObject("TestBoard");
            var bv = go.AddComponent<BoardView>();
            bv.SquareSize = 1f;
            Assert.That(bv.TryWorldToSquare(new Vector3(100, 100, 0), out _), Is.False);
            Object.DestroyImmediate(go);
        }
    }
}
