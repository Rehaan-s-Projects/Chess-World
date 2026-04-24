using NUnit.Framework;

namespace ChessWorld.Core.Tests
{
    public class EvaluationTests
    {
        [Test]
        public void Empty_ScoresZero()
        {
            Assert.That(Evaluation.Score(Board.Empty()), Is.EqualTo(0));
        }

        [Test]
        public void SymmetricStart_ScoresZero()
        {
            Assert.That(Evaluation.Score(Board.StartingPosition()), Is.EqualTo(0));
        }

        [Test]
        public void ExtraWhitePawn_IsPositive()
        {
            var b = Board.Empty()
                .WithPiece(new Square(0, 1), new Piece(Side.White, PieceType.Pawn));
            Assert.That(Evaluation.Score(b), Is.EqualTo(Evaluation.PawnValue));
        }

        [Test]
        public void ExtraBlackQueen_IsNegative()
        {
            var b = Board.Empty()
                .WithPiece(new Square(0, 0), new Piece(Side.Black, PieceType.Queen));
            Assert.That(Evaluation.Score(b), Is.EqualTo(-Evaluation.QueenValue));
        }
    }
}
