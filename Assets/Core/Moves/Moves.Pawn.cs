using System.Collections.Generic;

namespace ChessWorld.Core
{
    public static class PawnMoves
    {
        public static IEnumerable<Move> Generate(Board board, Square from)
        {
            var p = board[from];
            int dir = p.Side == Side.White ? 1 : -1;
            int startRank = p.Side == Side.White ? 1 : 6;
            int promoRank = p.Side == Side.White ? 7 : 0;

            // Forward one
            int oneRank = from.Rank + dir;
            if (Square.IsOnBoard(from.File, oneRank))
            {
                var one = new Square(from.File, oneRank);
                if (board[one].IsNone)
                {
                    if (oneRank == promoRank)
                        foreach (var m in PromotionMoves(from, one, capture: false)) yield return m;
                    else
                        yield return new Move(from, one, MoveKind.Normal);

                    // Forward two (only if one-forward was empty and we're on start rank)
                    if (from.Rank == startRank)
                    {
                        var two = new Square(from.File, from.Rank + 2 * dir);
                        if (board[two].IsNone)
                            yield return new Move(from, two, MoveKind.DoublePawnPush);
                    }
                }
            }

            // Diagonal captures
            foreach (var df in new[] { -1, 1 })
            {
                int nf = from.File + df;
                int nr = from.Rank + dir;
                if (!Square.IsOnBoard(nf, nr)) continue;
                var to = new Square(nf, nr);
                var victim = board[to];
                bool isEnPassant = board.EnPassantTarget.HasValue &&
                                   board.EnPassantTarget.Value == to;

                if (!victim.IsNone && victim.Side != p.Side)
                {
                    if (nr == promoRank)
                        foreach (var m in PromotionMoves(from, to, capture: true)) yield return m;
                    else
                        yield return new Move(from, to, MoveKind.Capture);
                }
                else if (isEnPassant)
                {
                    yield return new Move(from, to, MoveKind.EnPassant);
                }
            }
        }

        private static IEnumerable<Move> PromotionMoves(Square from, Square to, bool capture)
        {
            var kinds = new[] { PieceType.Queen, PieceType.Rook, PieceType.Bishop, PieceType.Knight };
            foreach (var k in kinds)
                yield return Move.Promotion(from, to, k, capture);
        }
    }
}
