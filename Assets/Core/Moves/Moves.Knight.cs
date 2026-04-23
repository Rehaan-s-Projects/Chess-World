using System.Collections.Generic;

namespace ChessWorld.Core
{
    public static class KnightMoves
    {
        private static readonly (int df, int dr)[] Offsets = new[]
        {
            (1, 2), (2, 1), (2, -1), (1, -2),
            (-1, -2), (-2, -1), (-2, 1), (-1, 2)
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
        }
    }
}
