using System;

namespace ChessWorld.Core
{
    public readonly struct Square : IEquatable<Square>
    {
        public int File { get; }
        public int Rank { get; }

        public Square(int file, int rank)
        {
            File = file;
            Rank = rank;
        }

        public int Index => Rank * 8 + File;

        public static Square FromIndex(int index) => new Square(index % 8, index / 8);

        public static bool IsOnBoard(int file, int rank) =>
            file >= 0 && file < 8 && rank >= 0 && rank < 8;

        public bool Equals(Square other) => File == other.File && Rank == other.Rank;
        public override bool Equals(object obj) => obj is Square s && Equals(s);
        public override int GetHashCode() => (File << 3) | Rank;
        public static bool operator ==(Square a, Square b) => a.Equals(b);
        public static bool operator !=(Square a, Square b) => !a.Equals(b);

        public string ToAlgebraic() => $"{(char)('a' + File)}{Rank + 1}";
        public override string ToString() => ToAlgebraic();
    }
}
