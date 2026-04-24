using System.Collections.Generic;

namespace ChessWorld.Core
{
    public static class KingMoves
    {
        private static readonly (int df, int dr)[] Offsets = {
            (1, 0), (-1, 0), (0, 1), (0, -1),
            (1, 1), (1, -1), (-1, 1), (-1, -1)
        };

        public static IEnumerable<Move> Generate(Board board, Square from)
        {
            var me = board[from].Side;
            foreach (var (df, dr) in Offsets)
            {
                int nf = from.File + df;
                int nr = from.Rank + dr;
                if (!Square.IsOnBoard(nf, nr)) continue;
                var to = new Square(nf, nr);
                var victim = board[to];
                if (victim.IsNone)
                    yield return new Move(from, to, MoveKind.Normal);
                else if (victim.Side != me)
                    yield return new Move(from, to, MoveKind.Capture);
            }

            // Castling
            int homeRank = me == Side.White ? 0 : 7;
            bool kingSide = me == Side.White
                ? board.CastlingRights.WhiteKingSide
                : board.CastlingRights.BlackKingSide;
            bool queenSide = me == Side.White
                ? board.CastlingRights.WhiteQueenSide
                : board.CastlingRights.BlackQueenSide;

            if (from.Rank == homeRank && from.File == 4)
            {
                if (kingSide &&
                    board[new Square(5, homeRank)].IsNone &&
                    board[new Square(6, homeRank)].IsNone &&
                    board[new Square(7, homeRank)].Type == PieceType.Rook &&
                    board[new Square(7, homeRank)].Side == me)
                {
                    yield return new Move(from, new Square(6, homeRank),
                        MoveKind.CastleKingSide);
                }
                if (queenSide &&
                    board[new Square(3, homeRank)].IsNone &&
                    board[new Square(2, homeRank)].IsNone &&
                    board[new Square(1, homeRank)].IsNone &&
                    board[new Square(0, homeRank)].Type == PieceType.Rook &&
                    board[new Square(0, homeRank)].Side == me)
                {
                    yield return new Move(from, new Square(2, homeRank),
                        MoveKind.CastleQueenSide);
                }
            }
        }
    }
}
