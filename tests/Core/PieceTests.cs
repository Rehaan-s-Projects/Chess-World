using NUnit.Framework;

namespace ChessWorld.Core.Tests
{
    public class PieceTests
    {
        [Test]
        public void None_HasTypeNone()
        {
            Assert.That(Piece.None.Type, Is.EqualTo(PieceType.None));
        }

        [Test]
        public void Constructor_StoresSideAndType()
        {
            var p = new Piece(Side.White, PieceType.Knight);
            Assert.That(p.Side, Is.EqualTo(Side.White));
            Assert.That(p.Type, Is.EqualTo(PieceType.Knight));
        }

        [Test]
        public void IsNone_TrueForNonePiece()
        {
            Assert.That(Piece.None.IsNone, Is.True);
            Assert.That(new Piece(Side.White, PieceType.Pawn).IsNone, Is.False);
        }

        [Test]
        public void Equality_ByValue()
        {
            Assert.That(new Piece(Side.Black, PieceType.Rook),
                Is.EqualTo(new Piece(Side.Black, PieceType.Rook)));
        }
    }

    public class CastlingRightsTests
    {
        [Test]
        public void All_ContainsEveryFlag()
        {
            var r = CastlingRights.All;
            Assert.That(r.WhiteKingSide, Is.True);
            Assert.That(r.WhiteQueenSide, Is.True);
            Assert.That(r.BlackKingSide, Is.True);
            Assert.That(r.BlackQueenSide, Is.True);
        }

        [Test]
        public void Without_ClearsSpecificFlag()
        {
            var r = CastlingRights.All.Without(Side.White, kingSide: true);
            Assert.That(r.WhiteKingSide, Is.False);
            Assert.That(r.WhiteQueenSide, Is.True);
            Assert.That(r.BlackKingSide, Is.True);
        }
    }
}
