using System.Linq;
using NUnit.Framework;

namespace ChessWorld.Core.Tests
{
    public class SlidingMoveTests
    {
        [Test]
        public void Rook_OnEmptyBoard_Has14Moves()
        {
            var b = Board.Empty().WithPiece(new Square(4, 4),
                new Piece(Side.White, PieceType.Rook));
            var moves = SlidingMoves.Generate(b, new Square(4, 4), PieceType.Rook).ToList();
            Assert.That(moves.Count, Is.EqualTo(14));
        }

        [Test]
        public void Bishop_OnEmptyBoard_Has13MovesFromD4()
        {
            var b = Board.Empty().WithPiece(new Square(3, 3),
                new Piece(Side.White, PieceType.Bishop));
            var moves = SlidingMoves.Generate(b, new Square(3, 3), PieceType.Bishop).ToList();
            Assert.That(moves.Count, Is.EqualTo(13));
        }

        [Test]
        public void Queen_OnEmptyBoard_Has27MovesFromD4()
        {
            var b = Board.Empty().WithPiece(new Square(3, 3),
                new Piece(Side.White, PieceType.Queen));
            var moves = SlidingMoves.Generate(b, new Square(3, 3), PieceType.Queen).ToList();
            Assert.That(moves.Count, Is.EqualTo(27));
        }

        [Test]
        public void Rook_StopsAtOwnPiece_DoesNotCapture()
        {
            var b = Board.Empty()
                .WithPiece(new Square(4, 4), new Piece(Side.White, PieceType.Rook))
                .WithPiece(new Square(4, 6), new Piece(Side.White, PieceType.Pawn));
            var moves = SlidingMoves.Generate(b, new Square(4, 4), PieceType.Rook).ToList();
            Assert.That(moves.Any(m => m.To == new Square(4, 5)), Is.True);
            Assert.That(moves.Any(m => m.To == new Square(4, 6)), Is.False);
            Assert.That(moves.Any(m => m.To == new Square(4, 7)), Is.False);
        }

        [Test]
        public void Rook_CapturesEnemyThenStops()
        {
            var b = Board.Empty()
                .WithPiece(new Square(4, 4), new Piece(Side.White, PieceType.Rook))
                .WithPiece(new Square(4, 6), new Piece(Side.Black, PieceType.Pawn));
            var moves = SlidingMoves.Generate(b, new Square(4, 4), PieceType.Rook).ToList();
            Assert.That(moves, Has.Member(new Move(new Square(4, 4), new Square(4, 6),
                MoveKind.Capture)));
            Assert.That(moves.Any(m => m.To == new Square(4, 7)), Is.False);
        }
    }
}
