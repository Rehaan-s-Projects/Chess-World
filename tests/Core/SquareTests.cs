using NUnit.Framework;

namespace ChessWorld.Core.Tests
{
    public class SquareTests
    {
        [Test]
        public void Construction_StoresFileAndRank()
        {
            var s = new Square(3, 5);
            Assert.That(s.File, Is.EqualTo(3));
            Assert.That(s.Rank, Is.EqualTo(5));
        }

        [Test]
        public void Index_IsRankTimesEightPlusFile()
        {
            Assert.That(new Square(0, 0).Index, Is.EqualTo(0));
            Assert.That(new Square(7, 0).Index, Is.EqualTo(7));
            Assert.That(new Square(0, 1).Index, Is.EqualTo(8));
            Assert.That(new Square(7, 7).Index, Is.EqualTo(63));
        }

        [Test]
        public void FromIndex_IsInverseOfIndex()
        {
            for (int i = 0; i < 64; i++)
            {
                var s = Square.FromIndex(i);
                Assert.That(s.Index, Is.EqualTo(i));
            }
        }

        [Test]
        [TestCase(-1, 0, false)]
        [TestCase(0, -1, false)]
        [TestCase(8, 0, false)]
        [TestCase(0, 8, false)]
        [TestCase(0, 0, true)]
        [TestCase(7, 7, true)]
        public void IsOnBoard_ChecksBounds(int file, int rank, bool expected)
        {
            Assert.That(Square.IsOnBoard(file, rank), Is.EqualTo(expected));
        }

        [Test]
        public void Equality_ByValue()
        {
            Assert.That(new Square(4, 2), Is.EqualTo(new Square(4, 2)));
            Assert.That(new Square(4, 2), Is.Not.EqualTo(new Square(4, 3)));
        }

        [Test]
        public void ToAlgebraic_MatchesChessNotation()
        {
            Assert.That(new Square(0, 0).ToAlgebraic(), Is.EqualTo("a1"));
            Assert.That(new Square(7, 7).ToAlgebraic(), Is.EqualTo("h8"));
            Assert.That(new Square(4, 3).ToAlgebraic(), Is.EqualTo("e4"));
        }
    }
}
