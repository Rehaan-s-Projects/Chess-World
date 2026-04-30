using System.Linq;
using NUnit.Framework;

namespace ChessWorld.Core.Tests
{
    public class KnightMoveTests
    {
        [Test]
        public void CenterKnight_HasEightMoves()
        {
            var b = Board.Empty().WithPiece(new Square(4, 4),
                new Piece(Side.White, PieceType.Knight));
            var moves = KnightMoves.Generate(b, new Square(4, 4)).ToList();
            Assert.That(moves.Count, Is.EqualTo(8));
        }

        [Test]
        public void CornerKnight_HasTwoMoves()
        {
            var b = Board.Empty().WithPiece(new Square(0, 0),
                new Piece(Side.White, PieceType.Knight));
            var moves = KnightMoves.Generate(b, new Square(0, 0)).ToList();
            Assert.That(moves.Count, Is.EqualTo(2));
        }

        [Test]
        public void Knight_CannotLandOnOwnPiece()
        {
            var b = Board.Empty()
                .WithPiece(new Square(4, 4), new Piece(Side.White, PieceType.Knight))
                .WithPiece(new Square(6, 5), new Piece(Side.White, PieceType.Pawn));
            var moves = KnightMoves.Generate(b, new Square(4, 4)).ToList();
            Assert.That(moves.Any(m => m.To == new Square(6, 5)), Is.False);
        }

        [Test]
        public void Knight_CapturesEnemy()
        {
            var b = Board.Empty()
                .WithPiece(new Square(4, 4), new Piece(Side.White, PieceType.Knight))
                .WithPiece(new Square(6, 5), new Piece(Side.Black, PieceType.Pawn));
            var moves = KnightMoves.Generate(b, new Square(4, 4)).ToList();
            Assert.That(moves, Has.Member(new Move(new Square(4, 4), new Square(6, 5),
                MoveKind.Capture)));
        }
    }
}
