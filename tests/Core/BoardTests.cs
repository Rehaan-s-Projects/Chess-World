using NUnit.Framework;

namespace ChessWorld.Core.Tests
{
    public class BoardTests
    {
        [Test]
        public void Empty_HasNoPieces()
        {
            var b = Board.Empty();
            for (int i = 0; i < 64; i++)
            {
                Assert.That(b[Square.FromIndex(i)].IsNone, Is.True);
            }
            Assert.That(b.SideToMove, Is.EqualTo(Side.White));
        }

        [Test]
        public void StartingPosition_HasCorrectBackRanks()
        {
            var b = Board.StartingPosition();
            Assert.That(b[new Square(0, 0)], Is.EqualTo(new Piece(Side.White, PieceType.Rook)));
            Assert.That(b[new Square(4, 0)], Is.EqualTo(new Piece(Side.White, PieceType.King)));
            Assert.That(b[new Square(4, 7)], Is.EqualTo(new Piece(Side.Black, PieceType.King)));
            Assert.That(b[new Square(0, 1)], Is.EqualTo(new Piece(Side.White, PieceType.Pawn)));
            Assert.That(b.CastlingRights.WhiteKingSide, Is.True);
            Assert.That(b.EnPassantTarget, Is.Null);
            Assert.That(b.SideToMove, Is.EqualTo(Side.White));
        }

        [Test]
        public void WithPiece_ReturnsNewBoardWithPieceSet()
        {
            var b = Board.Empty();
            var b2 = b.WithPiece(new Square(3, 3), new Piece(Side.Black, PieceType.Queen));
            Assert.That(b[new Square(3, 3)].IsNone, Is.True, "original unchanged");
            Assert.That(b2[new Square(3, 3)],
                Is.EqualTo(new Piece(Side.Black, PieceType.Queen)));
        }

        [Test]
        public void CountPieces_OfSide()
        {
            var b = Board.StartingPosition();
            Assert.That(b.CountPieces(Side.White), Is.EqualTo(16));
            Assert.That(b.CountPieces(Side.Black), Is.EqualTo(16));
        }
    }
}
