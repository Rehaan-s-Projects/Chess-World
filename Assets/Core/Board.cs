using System;

namespace ChessWorld.Core
{
    public sealed class Board
    {
        private readonly Piece[] _pieces;   // length 64

        public Side SideToMove { get; }
        public CastlingRights CastlingRights { get; }
        public Square? EnPassantTarget { get; }
        public int HalfmoveClock { get; }
        public int FullmoveNumber { get; }

        private Board(Piece[] pieces, Side sideToMove, CastlingRights rights,
                      Square? epTarget, int halfmove, int fullmove)
        {
            _pieces = pieces;
            SideToMove = sideToMove;
            CastlingRights = rights;
            EnPassantTarget = epTarget;
            HalfmoveClock = halfmove;
            FullmoveNumber = fullmove;
        }

        public Piece this[Square s] => _pieces[s.Index];

        public static Board Empty() =>
            new Board(new Piece[64], Side.White, CastlingRights.None, null, 0, 1);

        public static Board StartingPosition()
        {
            var p = new Piece[64];
            // Pawns
            for (int f = 0; f < 8; f++)
            {
                p[new Square(f, 1).Index] = new Piece(Side.White, PieceType.Pawn);
                p[new Square(f, 6).Index] = new Piece(Side.Black, PieceType.Pawn);
            }
            // Back ranks
            var order = new[]
            {
                PieceType.Rook, PieceType.Knight, PieceType.Bishop, PieceType.Queen,
                PieceType.King, PieceType.Bishop, PieceType.Knight, PieceType.Rook
            };
            for (int f = 0; f < 8; f++)
            {
                p[new Square(f, 0).Index] = new Piece(Side.White, order[f]);
                p[new Square(f, 7).Index] = new Piece(Side.Black, order[f]);
            }
            return new Board(p, Side.White, CastlingRights.All, null, 0, 1);
        }

        public Board WithPiece(Square s, Piece piece)
        {
            var next = (Piece[])_pieces.Clone();
            next[s.Index] = piece;
            return new Board(next, SideToMove, CastlingRights,
                             EnPassantTarget, HalfmoveClock, FullmoveNumber);
        }

        public Board With(Piece[] pieces, Side sideToMove, CastlingRights rights,
                          Square? epTarget, int halfmove, int fullmove) =>
            new Board(pieces, sideToMove, rights, epTarget, halfmove, fullmove);

        public Piece[] ClonePieces() => (Piece[])_pieces.Clone();

        public int CountPieces(Side side)
        {
            int c = 0;
            for (int i = 0; i < 64; i++)
                if (!_pieces[i].IsNone && _pieces[i].Side == side) c++;
            return c;
        }
    }
}
