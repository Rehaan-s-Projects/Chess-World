using System;

namespace ChessWorld.Core
{
    public readonly struct Piece : IEquatable<Piece>
    {
        public Side Side { get; }
        public PieceType Type { get; }

        public Piece(Side side, PieceType type)
        {
            Side = side;
            Type = type;
        }

        public static readonly Piece None = new Piece(Side.White, PieceType.None);

        public bool IsNone => Type == PieceType.None;

        public bool Equals(Piece other) => Side == other.Side && Type == other.Type;
        public override bool Equals(object obj) => obj is Piece p && Equals(p);
        public override int GetHashCode() => ((int)Side << 8) | (int)Type;
        public static bool operator ==(Piece a, Piece b) => a.Equals(b);
        public static bool operator !=(Piece a, Piece b) => !a.Equals(b);

        public override string ToString() =>
            IsNone ? "--" : $"{Side.ToString()[0]}{Type.ToString()[0]}";
    }
}
