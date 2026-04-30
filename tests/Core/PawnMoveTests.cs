using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ChessWorld.Core.Tests
{
    public class PawnMoveTests
    {
        private static Board WithOnlyPawn(Side side, Square at)
        {
            return Board.Empty().WithPiece(at, new Piece(side, PieceType.Pawn));
        }

        [Test]
        public void WhitePawn_OnSecondRank_CanPushOneOrTwo()
        {
            var b = WithOnlyPawn(Side.White, new Square(4, 1));
            var moves = PawnMoves.Generate(b, new Square(4, 1)).ToList();
            Assert.That(moves, Has.Member(new Move(new Square(4, 1), new Square(4, 2),
                MoveKind.Normal)));
            Assert.That(moves, Has.Member(new Move(new Square(4, 1), new Square(4, 3),
                MoveKind.DoublePawnPush)));
            Assert.That(moves.Count, Is.EqualTo(2));
        }

        [Test]
        public void WhitePawn_Blocked_CannotPush()
        {
            var b = WithOnlyPawn(Side.White, new Square(4, 1))
                .WithPiece(new Square(4, 2), new Piece(Side.Black, PieceType.Pawn));
            var moves = PawnMoves.Generate(b, new Square(4, 1)).ToList();
            Assert.That(moves.Any(m => m.To == new Square(4, 2)), Is.False);
            Assert.That(moves.Any(m => m.To == new Square(4, 3)), Is.False);
        }

        [Test]
        public void WhitePawn_CanCaptureDiagonally()
        {
            var b = WithOnlyPawn(Side.White, new Square(4, 3))
                .WithPiece(new Square(5, 4), new Piece(Side.Black, PieceType.Knight));
            var moves = PawnMoves.Generate(b, new Square(4, 3)).ToList();
            Assert.That(moves, Has.Member(new Move(new Square(4, 3), new Square(5, 4),
                MoveKind.Capture)));
        }

        [Test]
        public void WhitePawn_CannotCaptureOwn()
        {
            var b = WithOnlyPawn(Side.White, new Square(4, 3))
                .WithPiece(new Square(5, 4), new Piece(Side.White, PieceType.Knight));
            var moves = PawnMoves.Generate(b, new Square(4, 3)).ToList();
            Assert.That(moves.Any(m => m.To == new Square(5, 4)), Is.False);
        }

        [Test]
        public void WhitePawn_ReachingLastRank_Promotes()
        {
            var b = WithOnlyPawn(Side.White, new Square(0, 6));
            var moves = PawnMoves.Generate(b, new Square(0, 6)).ToList();
            Assert.That(moves.Count(m => m.Kind == MoveKind.Promotion), Is.EqualTo(4));
            Assert.That(moves.Select(m => m.PromotionTo),
                Is.EquivalentTo(new[] {
                    PieceType.Queen, PieceType.Rook, PieceType.Bishop, PieceType.Knight
                }));
        }

        [Test]
        public void BlackPawn_MovesDown()
        {
            var b = WithOnlyPawn(Side.Black, new Square(4, 6));
            var moves = PawnMoves.Generate(b, new Square(4, 6)).ToList();
            Assert.That(moves, Has.Member(new Move(new Square(4, 6), new Square(4, 5),
                MoveKind.Normal)));
            Assert.That(moves, Has.Member(new Move(new Square(4, 6), new Square(4, 4),
                MoveKind.DoublePawnPush)));
        }

        [Test]
        public void EnPassant_Generated_WhenTargetSet()
        {
            // White pawn on e5, black just played d7-d5, ep target d6.
            var pieces = Board.Empty().ClonePieces();
            pieces[new Square(4, 4).Index] = new Piece(Side.White, PieceType.Pawn);
            pieces[new Square(3, 4).Index] = new Piece(Side.Black, PieceType.Pawn);
            var b = Board.Empty().With(pieces, Side.White, CastlingRights.None,
                                        epTarget: new Square(3, 5), halfmove: 0, fullmove: 1);
            var moves = PawnMoves.Generate(b, new Square(4, 4)).ToList();
            Assert.That(moves, Has.Member(new Move(new Square(4, 4), new Square(3, 5),
                MoveKind.EnPassant)));
        }
    }
}
