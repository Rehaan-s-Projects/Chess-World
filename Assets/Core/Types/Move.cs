using System;

namespace ChessWorld.Core
{
    public readonly struct Move : IEquatable<Move>
    {
        public Square From { get; }
        public Square To { get; }
        public MoveKind Kind { get; }
        public PieceType PromotionTo { get; }

        public Move(Square from, Square to, MoveKind kind)
        {
            From = from;
            To = to;
            Kind = kind;
            PromotionTo = PieceType.None;
        }

        private Move(Square from, Square to, MoveKind kind, PieceType promotionTo)
        {
            From = from;
            To = to;
            Kind = kind;
            PromotionTo = promotionTo;
        }

        public static Move Promotion(Square from, Square to, PieceType to_, bool isCapture)
        {
            return new Move(from, to,
                isCapture ? MoveKind.PromotionCapture : MoveKind.Promotion, to_);
        }

        public bool IsCapture =>
            Kind == MoveKind.Capture || Kind == MoveKind.EnPassant ||
            Kind == MoveKind.PromotionCapture;

        public bool Equals(Move other) =>
            From == other.From && To == other.To &&
            Kind == other.Kind && PromotionTo == other.PromotionTo;
        public override bool Equals(object obj) => obj is Move m && Equals(m);
        public override int GetHashCode() =>
            (From.Index << 10) | (To.Index << 4) | (int)Kind;
        public static bool operator ==(Move a, Move b) => a.Equals(b);
        public static bool operator !=(Move a, Move b) => !a.Equals(b);

        public override string ToString() => $"{From}-{To}";
    }
}
