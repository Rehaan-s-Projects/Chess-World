using System.Linq;
using NUnit.Framework;

namespace ChessWorld.Core.Tests
{
    public class MoveGeneratorTests
    {
        [Test]
        public void StartingPosition_White_Has20Moves()
        {
            var b = Board.StartingPosition();
            var moves = MoveGenerator.LegalMoves(b).ToList();
            // 16 pawn moves (8 single pushes + 8 double pushes) + 4 knight moves
            Assert.That(moves.Count, Is.EqualTo(20));
        }

        [Test]
        public void EmptyBoard_ReturnsNoMoves()
        {
            var b = Board.Empty();
            var moves = MoveGenerator.LegalMoves(b).ToList();
            Assert.That(moves, Is.Empty);
        }
    }
}
