using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessWorld.Core
{
    public sealed class GameEngine
    {
        private readonly MinimaxAi _ai;

        public Board Board { get; private set; } = Board.Empty();
        public GameResult Result { get; private set; } = GameResult.InProgress;

        public event Action<Move, Board> MovePlayed;
        public event Action<GameResult> GameEnded;

        public GameEngine(int aiDepth = 3)
        {
            _ai = new MinimaxAi(aiDepth);
        }

        public void StartNewGame()
        {
            Board = Board.StartingPosition();
            Result = GameResult.InProgress;
        }

        public void StartNewGame(Board initialPosition)
        {
            Board = initialPosition;
            Result = GameResult.InProgress;
        }

        public IReadOnlyList<Move> LegalMovesFrom(Square from) =>
            MoveGenerator.LegalMovesFrom(Board, from).ToList();

        public bool TryPlayerMove(Square from, Square to,
                                  PieceType promotion = PieceType.Queen)
        {
            var legal = MoveGenerator.LegalMovesFrom(Board, from).ToList();
            var candidates = legal.Where(m => m.To == to).ToList();
            if (candidates.Count == 0) return false;

            var move = candidates.FirstOrDefault(m => m.PromotionTo == promotion);
            if (move.Equals(default(Move))) move = candidates[0];

            return ApplyMove(move);
        }

        public Move? ComputeAiMove() => _ai.ChooseMove(Board);

        public bool ApplyMove(Move move)
        {
            try
            {
                Board = Rules.Apply(Board, move);
            }
            catch (InvalidMoveException)
            {
                return false;
            }
            MovePlayed?.Invoke(move, Board);
            Result = Rules.Evaluate(Board);
            if (Result != GameResult.InProgress) GameEnded?.Invoke(Result);
            return true;
        }
    }
}
