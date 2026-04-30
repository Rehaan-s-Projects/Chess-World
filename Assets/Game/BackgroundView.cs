using UnityEngine;

namespace ChessWorld.Game
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class BackgroundView : MonoBehaviour
    {
        public SpriteCatalog Catalog;
        [SerializeField] private WorldState _state = WorldState.Decayed;

        private SpriteRenderer _sr;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            Render();
        }

        public void SetWorldState(WorldState state)
        {
            _state = state;
            Render();
        }

        private void Render()
        {
            if (Catalog == null)
            {
                Debug.LogError("BackgroundView: SpriteCatalog not assigned.");
                return;
            }
            // Phase 1 always renders Decayed; Phase 2 adds the Cured state transition.
            var sprite = Catalog.DecayedBackground ?? Catalog.FallbackBackground;
            if (sprite == null)
            {
                Debug.LogError("BackgroundView: no background sprite available.");
                _sr.color = new Color(0.1f, 0.1f, 0.12f);  // dark slate solid color
                return;
            }
            _sr.sprite = sprite;
        }
    }
}
