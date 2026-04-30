using System.Collections;
using UnityEngine;
using ChessWorld.Core;

namespace ChessWorld.Game
{
    public sealed class PieceView : MonoBehaviour
    {
        public Side Side;
        public PieceType Type;
        public Square Square;

        public const float MoveDurationSeconds = 0.25f;
        public const float CaptureDurationSeconds = 0.3f;

        private SpriteRenderer _sr;

        public void Configure(Side side, PieceType type, Square sq, Sprite sprite)
        {
            Side = side;
            Type = type;
            Square = sq;
            if (_sr == null) _sr = gameObject.AddComponent<SpriteRenderer>();
            if (sprite == null)
            {
                Debug.LogError($"PieceView: missing sprite for {side} {type}");
                _sr.color = Color.magenta;
            }
            else
            {
                _sr.sprite = sprite;
                _sr.color = Color.white;
            }
            _sr.sortingOrder = 5;
            // Inset so the underlying square color frames each piece. Phase 2 sprites
            // will have their own outlines/silhouettes; this is a placeholder concession.
            transform.localScale = Vector3.one * 0.8f;
        }

        public IEnumerator AnimateMoveTo(Vector3 worldTarget, Square newSquare)
        {
            Vector3 start = transform.localPosition;
            float t = 0f;
            while (t < MoveDurationSeconds)
            {
                t += Time.deltaTime;
                float u = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / MoveDurationSeconds));
                transform.localPosition = Vector3.Lerp(start, worldTarget, u);
                yield return null;
            }
            transform.localPosition = worldTarget;
            Square = newSquare;
        }

        public IEnumerator AnimateCaptureAndDestroy()
        {
            if (_sr == null) { Destroy(gameObject); yield break; }
            float t = 0f;
            Color baseColor = _sr.color;
            while (t < CaptureDurationSeconds)
            {
                t += Time.deltaTime;
                float u = Mathf.Clamp01(t / CaptureDurationSeconds);
                _sr.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f - u);
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
