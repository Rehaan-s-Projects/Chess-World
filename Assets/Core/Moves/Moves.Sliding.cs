using System.Collections.Generic;

namespace ChessWorld.Core
{
    public static class SlidingMoves
    {
        private static readonly (int df, int dr)[] Rook = {
            (1, 0), (-1, 0), (0, 1), (0, -1)
        };
        private static readonly (int df, int dr)[] Bishop = {
            (1, 1), (1, -1), (-1, 1), (-1, -1)
        };
        private static readonly (int df, int dr)[] Queen = {
            (1, 0), (-1, 0), (0, 1), (0, -1),
            (1, 1), (1, -1), (-1, 1), (-1, -1)
        };

        public static IEnumerable<Move> Generate(Board board, Square from, PieceType type)
        {
            var dirs = type switch
            {
                PieceType.Rook => Rook,
                PieceType.Bishop => Bishop,
                PieceType.Queen => Queen,
                _ => throw new System.ArgumentException($"Not a sliding piece: {type}")
            };

            var me = board[from].Side;
            foreach (var (df, dr) in dirs)
            {
                int f = from.File + df;
                int r = from.Rank + dr;
                while (Square.IsOnBoard(f, r))
                {
                    var to = new Square(f, r);
                    var victim = board[to];
                    if (victim.IsNone)
                    {
                        yield return new Move(from, to, MoveKind.Normal);
                    }
                    else
                    {
                        if (victim.Side != me)
                            yield return new Move(from, to, MoveKind.Capture);
                        break;
                    }
                    f += df;
                    r += dr;
                }
            }
        }
    }
}
