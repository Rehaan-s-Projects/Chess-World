namespace ChessWorld.Core
{
    public static class Evaluation
    {
        public const int PawnValue = 100;
        public const int KnightValue = 300;
        public const int BishopValue = 300;
        public const int RookValue = 500;
        public const int QueenValue = 900;
        public const int KingValue = 100000;

        public static int Score(Board board)
        {
            int score = 0;
            for (int i = 0; i < 64; i++)
            {
                var p = board[Square.FromIndex(i)];
                if (p.IsNone) continue;
                int v = Value(p.Type);
                score += p.Side == Side.White ? v : -v;
            }
            return score;
        }

        public static int Value(PieceType t) => t switch
        {
            PieceType.Pawn => PawnValue,
            PieceType.Knight => KnightValue,
            PieceType.Bishop => BishopValue,
            PieceType.Rook => RookValue,
            PieceType.Queen => QueenValue,
            PieceType.King => KingValue,
            _ => 0
        };
    }
}
