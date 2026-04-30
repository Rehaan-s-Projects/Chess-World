using System.Collections.Generic;

namespace ChessWorld.Core
{
    public static class MoveGenerator
    {
        public static IEnumerable<Move> LegalMoves(Board board)
        {
            for (int i = 0; i < 64; i++)
            {
                var sq = Square.FromIndex(i);
                var p = board[sq];
                if (p.IsNone || p.Side != board.SideToMove) continue;

                foreach (var m in LegalMovesFrom(board, sq))
                    yield return m;
            }
        }

        public static IEnumerable<Move> LegalMovesFrom(Board board, Square from)
        {
            var p = board[from];
            if (p.IsNone || p.Side != board.SideToMove) yield break;

            IEnumerable<Move> source = p.Type switch
            {
                PieceType.Pawn => PawnMoves.Generate(board, from),
                PieceType.Knight => KnightMoves.Generate(board, from),
                PieceType.Bishop => SlidingMoves.Generate(board, from, PieceType.Bishop),
                PieceType.Rook => SlidingMoves.Generate(board, from, PieceType.Rook),
                PieceType.Queen => SlidingMoves.Generate(board, from, PieceType.Queen),
                PieceType.King => KingMoves.Generate(board, from),
                _ => System.Array.Empty<Move>()
            };
            foreach (var m in source) yield return m;
        }
    }
}
