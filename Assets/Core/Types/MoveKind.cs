namespace ChessWorld.Core
{
    public enum MoveKind : byte
    {
        Normal = 0,
        Capture = 1,
        DoublePawnPush = 2,
        EnPassant = 3,
        CastleKingSide = 4,
        CastleQueenSide = 5,
        Promotion = 6,
        PromotionCapture = 7
    }
}
