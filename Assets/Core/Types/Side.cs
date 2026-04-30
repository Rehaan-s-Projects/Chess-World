namespace ChessWorld.Core
{
    public enum Side : byte
    {
        White = 0,
        Black = 1
    }

    public static class SideExtensions
    {
        public static Side Opposite(this Side s) => s == Side.White ? Side.Black : Side.White;
    }
}
