namespace ChessWorld.Core
{
    public readonly struct CastlingRights
    {
        public bool WhiteKingSide { get; }
        public bool WhiteQueenSide { get; }
        public bool BlackKingSide { get; }
        public bool BlackQueenSide { get; }

        public CastlingRights(bool wk, bool wq, bool bk, bool bq)
        {
            WhiteKingSide = wk;
            WhiteQueenSide = wq;
            BlackKingSide = bk;
            BlackQueenSide = bq;
        }

        public static readonly CastlingRights All = new CastlingRights(true, true, true, true);
        public static readonly CastlingRights None = new CastlingRights(false, false, false, false);

        public CastlingRights Without(Side side, bool kingSide)
        {
            if (side == Side.White)
            {
                return new CastlingRights(
                    kingSide ? false : WhiteKingSide,
                    kingSide ? WhiteQueenSide : false,
                    BlackKingSide, BlackQueenSide);
            }
            return new CastlingRights(
                WhiteKingSide, WhiteQueenSide,
                kingSide ? false : BlackKingSide,
                kingSide ? BlackQueenSide : false);
        }

        public CastlingRights WithoutAll(Side side) =>
            side == Side.White
                ? new CastlingRights(false, false, BlackKingSide, BlackQueenSide)
                : new CastlingRights(WhiteKingSide, WhiteQueenSide, false, false);
    }
}
