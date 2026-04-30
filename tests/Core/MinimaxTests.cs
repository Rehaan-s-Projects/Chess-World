using NUnit.Framework;

namespace ChessWorld.Core.Tests
{
    public class MinimaxTests
    {
        [Test]
        public void FindsFreeCapture()
        {
            // White knight at b1; Black hanging queen at c3. Best move = Nxc3.
            var pieces = Board.Empty().ClonePieces();
            pieces[new Square(1, 0).Index] = new Piece(Side.White, PieceType.Knight);
            pieces[new Square(2, 2).Index] = new Piece(Side.Black, PieceType.Queen);
            pieces[new Square(4, 0).Index] = new Piece(Side.White, PieceType.King);
            pieces[new Square(4, 7).Index] = new Piece(Side.Black, PieceType.King);
            var b = Board.Empty().With(pieces, Side.White, CastlingRights.None, null, 0, 1);

            var ai = new MinimaxAi(depth: 3);
            var move = ai.ChooseMove(b);
            Assert.That(move.Value.From, Is.EqualTo(new Square(1, 0)));
            Assert.That(move.Value.To, Is.EqualTo(new Square(2, 2)));
        }

        [Test]
        public void NoMoves_ReturnsNull()
        {
            var b = Board.Empty();
            var ai = new MinimaxAi(depth: 2);
            Assert.That(ai.ChooseMove(b), Is.Null);
        }
    }
}
