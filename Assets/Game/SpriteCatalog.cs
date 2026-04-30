using UnityEngine;
using ChessWorld.Core;

namespace ChessWorld.Game
{
    [CreateAssetMenu(menuName = "ChessWorld/Sprite Catalog", fileName = "SpriteCatalog")]
    public sealed class SpriteCatalog : ScriptableObject
    {
        [System.Serializable]
        public class Entry
        {
            public Side Side;
            public PieceType Type;
            public Sprite Sprite;
        }

        public Entry[] PieceSprites;
        public Sprite LightSquare;
        public Sprite DarkSquare;
        public Sprite HighlightSquare;
        public Sprite LegalMoveDot;
        public Sprite DecayedBackground;   // Rehaan's image
        public Sprite FallbackBackground;  // plain gradient

        public Sprite GetPiece(Side side, PieceType type)
        {
            foreach (var e in PieceSprites)
                if (e.Side == side && e.Type == type) return e.Sprite;
            return null;  // caller handles missing sprite
        }
    }
}
