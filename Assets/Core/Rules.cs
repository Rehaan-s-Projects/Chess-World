using System.Linq;

namespace ChessWorld.Core
{
    public static class Rules
    {
        public static Board Apply(Board board, Move move)
        {
            if (!MoveGenerator.LegalMovesFrom(board, move.From).Contains(move))
                throw new InvalidMoveException(move, "not in legal move set");

            var pieces = board.ClonePieces();
            var mover = pieces[move.From.Index];
            pieces[move.From.Index] = Piece.None;

            // Handle promotion piece type
            var placed = move.Kind == MoveKind.Promotion || move.Kind == MoveKind.PromotionCapture
                ? new Piece(mover.Side, move.PromotionTo)
                : mover;
            pieces[move.To.Index] = placed;

            // En passant: remove captured pawn from its actual square (one rank behind To)
            if (move.Kind == MoveKind.EnPassant)
            {
                int captureRank = mover.Side == Side.White ? move.To.Rank - 1 : move.To.Rank + 1;
                pieces[new Square(move.To.File, captureRank).Index] = Piece.None;
            }

            // Castling: move the rook
            if (move.Kind == MoveKind.CastleKingSide)
            {
                int r = move.From.Rank;
                pieces[new Square(5, r).Index] = new Piece(mover.Side, PieceType.Rook);
                pieces[new Square(7, r).Index] = Piece.None;
            }
            else if (move.Kind == MoveKind.CastleQueenSide)
            {
                int r = move.From.Rank;
                pieces[new Square(3, r).Index] = new Piece(mover.Side, PieceType.Rook);
                pieces[new Square(0, r).Index] = Piece.None;
            }

            // Update castling rights
            var rights = board.CastlingRights;
            if (mover.Type == PieceType.King)
                rights = rights.WithoutAll(mover.Side);
            if (mover.Type == PieceType.Rook)
            {
                int homeRank = mover.Side == Side.White ? 0 : 7;
                if (move.From.Rank == homeRank && move.From.File == 0)
                    rights = rights.Without(mover.Side, kingSide: false);
                if (move.From.Rank == homeRank && move.From.File == 7)
                    rights = rights.Without(mover.Side, kingSide: true);
            }
            // If we captured a rook on its home square, that side loses the right
            if (move.IsCapture)
            {
                if (move.To.File == 0 && move.To.Rank == 0)
                    rights = rights.Without(Side.White, kingSide: false);
                if (move.To.File == 7 && move.To.Rank == 0)
                    rights = rights.Without(Side.White, kingSide: true);
                if (move.To.File == 0 && move.To.Rank == 7)
                    rights = rights.Without(Side.Black, kingSide: false);
                if (move.To.File == 7 && move.To.Rank == 7)
                    rights = rights.Without(Side.Black, kingSide: true);
            }

            // En passant target for next move
            Square? epTarget = null;
            if (move.Kind == MoveKind.DoublePawnPush)
            {
                int epRank = mover.Side == Side.White ? move.To.Rank - 1 : move.To.Rank + 1;
                epTarget = new Square(move.To.File, epRank);
            }

            int halfmove = (mover.Type == PieceType.Pawn || move.IsCapture)
                ? 0 : board.HalfmoveClock + 1;
            int fullmove = mover.Side == Side.Black
                ? board.FullmoveNumber + 1 : board.FullmoveNumber;

            return board.With(pieces, board.SideToMove.Opposite(), rights,
                              epTarget, halfmove, fullmove);
        }

        public static GameResult Evaluate(Board board)
        {
            bool whiteHas = board.CountPieces(Side.White) > 0;
            bool blackHas = board.CountPieces(Side.Black) > 0;
            if (!whiteHas) return GameResult.BlackWins;
            if (!blackHas) return GameResult.WhiteWins;
            return GameResult.InProgress;
        }
    }
}
