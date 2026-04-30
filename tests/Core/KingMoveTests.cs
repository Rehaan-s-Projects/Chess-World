using System.Linq;
using NUnit.Framework;

namespace ChessWorld.Core.Tests
{
    public class KingMoveTests
    {
        [Test]
        public void CenterKing_HasEightMoves()
        {
            var b = Board.Empty().WithPiece(new Square(4, 4),
                new Piece(Side.White, PieceType.King));
            var moves = KingMoves.Generate(b, new Square(4, 4)).ToList();
            Assert.That(moves.Count, Is.EqualTo(8));
        }

        [Test]
        public void King_Captures_Enemy()
        {
            var b = Board.Empty()
                .WithPiece(new Square(4, 4), new Piece(Side.White, PieceType.King))
                .WithPiece(new Square(5, 4), new Piece(Side.Black, PieceType.Pawn));
            var moves = KingMoves.Generate(b, new Square(4, 4)).ToList();
            Assert.That(moves, Has.Member(new Move(new Square(4, 4), new Square(5, 4),
                MoveKind.Capture)));
        }

        [Test]
        public void WhiteCastling_BothSides_WhenPathClearAndRightsExist()
        {
            var pieces = Board.Empty().ClonePieces();
            pieces[new Square(4, 0).Index] = new Piece(Side.White, PieceType.King);
            pieces[new Square(0, 0).Index] = new Piece(Side.White, PieceType.Rook);
            pieces[new Square(7, 0).Index] = new Piece(Side.White, PieceType.Rook);
            var b = Board.Empty().With(pieces, Side.White, CastlingRights.All, null, 0, 1);

            var moves = KingMoves.Generate(b, new Square(4, 0)).ToList();
            Assert.That(moves, Has.Member(new Move(new Square(4, 0), new Square(6, 0),
                MoveKind.CastleKingSide)));
            Assert.That(moves, Has.Member(new Move(new Square(4, 0), new Square(2, 0),
                MoveKind.CastleQueenSide)));
        }

        [Test]
        public void WhiteCastling_BlockedByPiece_Disallowed()
        {
            var pieces = Board.Empty().ClonePieces();
            pieces[new Square(4, 0).Index] = new Piece(Side.White, PieceType.King);
            pieces[new Square(7, 0).Index] = new Piece(Side.White, PieceType.Rook);
            pieces[new Square(5, 0).Index] = new Piece(Side.White, PieceType.Bishop);
            var b = Board.Empty().With(pieces, Side.White, CastlingRights.All, null, 0, 1);

            var moves = KingMoves.Generate(b, new Square(4, 0)).ToList();
            Assert.That(moves.Any(m => m.Kind == MoveKind.CastleKingSide), Is.False);
        }

        [Test]
        public void WhiteCastling_WithoutRights_Disallowed()
        {
            var pieces = Board.Empty().ClonePieces();
            pieces[new Square(4, 0).Index] = new Piece(Side.White, PieceType.King);
            pieces[new Square(7, 0).Index] = new Piece(Side.White, PieceType.Rook);
            var b = Board.Empty().With(pieces, Side.White, CastlingRights.None, null, 0, 1);

            var moves = KingMoves.Generate(b, new Square(4, 0)).ToList();
            Assert.That(moves.Any(m => m.Kind == MoveKind.CastleKingSide), Is.False);
        }
    }
}
