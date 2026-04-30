using System.Linq;
using NUnit.Framework;

namespace ChessWorld.Core.Tests
{
    public class RulesTests
    {
        [Test]
        public void Apply_Normal_MovesPiece()
        {
            var b = Board.StartingPosition();
            var move = new Move(new Square(4, 1), new Square(4, 3), MoveKind.DoublePawnPush);
            var b2 = Rules.Apply(b, move);
            Assert.That(b2[new Square(4, 1)].IsNone, Is.True);
            Assert.That(b2[new Square(4, 3)].Type, Is.EqualTo(PieceType.Pawn));
            Assert.That(b2.SideToMove, Is.EqualTo(Side.Black));
        }

        [Test]
        public void Apply_DoublePawnPush_SetsEnPassantTarget()
        {
            var b = Board.StartingPosition();
            var move = new Move(new Square(4, 1), new Square(4, 3), MoveKind.DoublePawnPush);
            var b2 = Rules.Apply(b, move);
            Assert.That(b2.EnPassantTarget, Is.EqualTo(new Square(4, 2)));
        }

        [Test]
        public void Apply_Promotion_PlacesPromotedPiece()
        {
            var pieces = Board.Empty().ClonePieces();
            pieces[new Square(0, 6).Index] = new Piece(Side.White, PieceType.Pawn);
            var b = Board.Empty().With(pieces, Side.White, CastlingRights.None, null, 0, 1);
            var move = Move.Promotion(new Square(0, 6), new Square(0, 7),
                                      PieceType.Queen, isCapture: false);
            var b2 = Rules.Apply(b, move);
            Assert.That(b2[new Square(0, 7)],
                Is.EqualTo(new Piece(Side.White, PieceType.Queen)));
        }

        [Test]
        public void Apply_Castling_MovesKingAndRook()
        {
            var pieces = Board.Empty().ClonePieces();
            pieces[new Square(4, 0).Index] = new Piece(Side.White, PieceType.King);
            pieces[new Square(7, 0).Index] = new Piece(Side.White, PieceType.Rook);
            var b = Board.Empty().With(pieces, Side.White, CastlingRights.All, null, 0, 1);

            var b2 = Rules.Apply(b,
                new Move(new Square(4, 0), new Square(6, 0), MoveKind.CastleKingSide));
            Assert.That(b2[new Square(6, 0)].Type, Is.EqualTo(PieceType.King));
            Assert.That(b2[new Square(5, 0)].Type, Is.EqualTo(PieceType.Rook));
            Assert.That(b2[new Square(4, 0)].IsNone, Is.True);
            Assert.That(b2[new Square(7, 0)].IsNone, Is.True);
            Assert.That(b2.CastlingRights.WhiteKingSide, Is.False);
            Assert.That(b2.CastlingRights.WhiteQueenSide, Is.False);
        }

        [Test]
        public void Apply_EnPassant_CapturesVictimOnOriginRank()
        {
            var pieces = Board.Empty().ClonePieces();
            pieces[new Square(4, 4).Index] = new Piece(Side.White, PieceType.Pawn);
            pieces[new Square(3, 4).Index] = new Piece(Side.Black, PieceType.Pawn);
            var b = Board.Empty().With(pieces, Side.White, CastlingRights.None,
                                        new Square(3, 5), 0, 1);
            var b2 = Rules.Apply(b,
                new Move(new Square(4, 4), new Square(3, 5), MoveKind.EnPassant));
            Assert.That(b2[new Square(3, 5)].Type, Is.EqualTo(PieceType.Pawn));
            Assert.That(b2[new Square(3, 4)].IsNone, Is.True,
                "captured pawn removed from rank 4");
        }

        [Test]
        public void Apply_KingMove_ClearsBothCastlingRights()
        {
            var pieces = Board.Empty().ClonePieces();
            pieces[new Square(4, 0).Index] = new Piece(Side.White, PieceType.King);
            var b = Board.Empty().With(pieces, Side.White, CastlingRights.All, null, 0, 1);
            var b2 = Rules.Apply(b,
                new Move(new Square(4, 0), new Square(4, 1), MoveKind.Normal));
            Assert.That(b2.CastlingRights.WhiteKingSide, Is.False);
            Assert.That(b2.CastlingRights.WhiteQueenSide, Is.False);
        }

        [Test]
        public void Apply_IllegalMove_Throws()
        {
            var b = Board.StartingPosition();
            var bogus = new Move(new Square(4, 1), new Square(4, 5), MoveKind.Normal);
            Assert.Throws<InvalidMoveException>(() => Rules.Apply(b, bogus));
        }

        [Test]
        public void Evaluate_WhiteLoss_WhenWhiteHasNoPieces()
        {
            var b = Board.Empty()
                .WithPiece(new Square(0, 0), new Piece(Side.Black, PieceType.Rook));
            Assert.That(Rules.Evaluate(b), Is.EqualTo(GameResult.BlackWins));
        }

        [Test]
        public void Evaluate_InProgress_WhenBothSidesHavePieces()
        {
            Assert.That(Rules.Evaluate(Board.StartingPosition()),
                Is.EqualTo(GameResult.InProgress));
        }
    }
}
