using System.Collections.Generic;

namespace ChessWorld.Core
{
    public sealed class MinimaxAi
    {
        private readonly int _depth;
        public MinimaxAi(int depth = 3) { _depth = depth; }

        // Returns null if no legal moves.
        public Move? ChooseMove(Board board)
        {
            int bestScore = int.MinValue + 1;
            Move? best = null;
            int sign = board.SideToMove == Side.White ? 1 : -1;

            foreach (var m in MoveGenerator.LegalMoves(board))
            {
                var next = Rules.Apply(board, m);
                int score = -Negamax(next, _depth - 1, int.MinValue + 1, int.MaxValue - 1, -sign);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = m;
                }
            }
            return best;
        }

        private int Negamax(Board board, int depth, int alpha, int beta, int sign)
        {
            var result = Rules.Evaluate(board);
            if (result != GameResult.InProgress)
                return sign * TerminalScore(result);
            if (depth == 0) return sign * Evaluation.Score(board);

            int best = int.MinValue + 1;
            bool anyMoves = false;
            foreach (var m in MoveGenerator.LegalMoves(board))
            {
                anyMoves = true;
                var next = Rules.Apply(board, m);
                int score = -Negamax(next, depth - 1, -beta, -alpha, -sign);
                if (score > best) best = score;
                if (best > alpha) alpha = best;
                if (alpha >= beta) break;
            }
            if (!anyMoves) return sign * Evaluation.Score(board);
            return best;
        }

        private static int TerminalScore(GameResult r) => r switch
        {
            GameResult.WhiteWins => Evaluation.KingValue,
            GameResult.BlackWins => -Evaluation.KingValue,
            _ => 0
        };
    }
}
