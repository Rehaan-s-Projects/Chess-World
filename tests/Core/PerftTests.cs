using NUnit.Framework;

namespace ChessWorld.Core.Tests
{
    public class PerftTests
    {
        [Test]
        [TestCase(1, 20L)]
        [TestCase(2, 400L)]
        [TestCase(3, 8902L)]
        public void Perft_StartingPosition(int depth, long expected)
        {
            var b = Board.StartingPosition();
            long actual = Perft(b, depth);
            Assert.That(actual, Is.EqualTo(expected),
                $"perft({depth}) mismatch. Classical chess value is {expected}. " +
                $"If our Eliminate ruleset legitimately diverges, update the expected " +
                $"value to {actual} and add a comment explaining the divergence.");
        }

        public static long Perft(Board b, int depth)
        {
            if (depth == 0) return 1;
            long count = 0;
            foreach (var m in MoveGenerator.LegalMoves(b))
                count += Perft(Rules.Apply(b, m), depth - 1);
            return count;
        }
    }
}
