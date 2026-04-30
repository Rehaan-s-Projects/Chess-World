using System.Collections.Generic;
using NUnit.Framework;

namespace ChessWorld.Core.Tests
{
    public class GameEngineTests
    {
        [Test]
        public void StartNewGame_SetsStartingPosition()
        {
            var eng = new GameEngine();
            eng.StartNewGame();
            Assert.That(eng.Board[new Square(0, 0)].Type, Is.EqualTo(PieceType.Rook));
            Assert.That(eng.Result, Is.EqualTo(GameResult.InProgress));
        }

        [Test]
        public void TryPlayerMove_LegalMove_AppliesAndFiresEvent()
        {
            var eng = new GameEngine();
            eng.StartNewGame();
            var fired = new List<Move>();
            eng.MovePlayed += (m, b) => fired.Add(m);

            bool ok = eng.TryPlayerMove(new Square(4, 1), new Square(4, 3));
            Assert.That(ok, Is.True);
            Assert.That(fired, Has.Count.EqualTo(1));
            Assert.That(fired[0].To, Is.EqualTo(new Square(4, 3)));
        }

        [Test]
        public void TryPlayerMove_IllegalMove_ReturnsFalseAndDoesNotApply()
        {
            var eng = new GameEngine();
            eng.StartNewGame();
            bool ok = eng.TryPlayerMove(new Square(4, 1), new Square(4, 5));
            Assert.That(ok, Is.False);
            Assert.That(eng.Board[new Square(4, 1)].Type, Is.EqualTo(PieceType.Pawn));
        }

        [Test]
        public void LegalMovesFrom_DelegatesToGenerator()
        {
            var eng = new GameEngine();
            eng.StartNewGame();
            var moves = eng.LegalMovesFrom(new Square(4, 1));
            Assert.That(moves, Is.Not.Empty);
        }
    }
}
