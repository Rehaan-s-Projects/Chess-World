using System.Collections.Generic;
using UnityEngine;
using ChessWorld.Core;

namespace ChessWorld.Game
{
    public sealed class BoardView : MonoBehaviour
    {
        public SpriteCatalog Catalog;
        public float SquareSize = 1f;

        private readonly Dictionary<Square, GameObject> _squares = new Dictionary<Square, GameObject>();
        private readonly Dictionary<Square, GameObject> _highlights = new Dictionary<Square, GameObject>();

        private void Awake()
        {
            SpawnSquares();
        }

        public Vector3 WorldOf(Square s)
        {
            // Center the 8x8 board on origin.
            float x = (s.File - 3.5f) * SquareSize;
            float y = (s.Rank - 3.5f) * SquareSize;
            return new Vector3(x, y, 0);
        }

        public bool TryWorldToSquare(Vector3 world, out Square result)
        {
            int f = Mathf.RoundToInt(world.x / SquareSize + 3.5f);
            int r = Mathf.RoundToInt(world.y / SquareSize + 3.5f);
            if (!Square.IsOnBoard(f, r))
            {
                result = default;
                return false;
            }
            result = new Square(f, r);
            return true;
        }

        public void HighlightSelection(Square? s)
        {
            ClearHighlights("selection");
            if (!s.HasValue) return;
            SpawnHighlight(s.Value, "selection", new Color(1f, 0.9f, 0.4f, 0.55f));
        }

        public void HighlightLegalMoves(IEnumerable<Square> squares)
        {
            ClearHighlights("legal");
            foreach (var sq in squares)
                SpawnHighlight(sq, "legal", new Color(0.4f, 1f, 0.4f, 0.35f));
        }

        public void ClearAllHighlights()
        {
            ClearHighlights("selection");
            ClearHighlights("legal");
        }

        private void SpawnSquares()
        {
            for (int i = 0; i < 64; i++)
            {
                var sq = Square.FromIndex(i);
                var go = new GameObject($"Sq_{sq.ToAlgebraic()}");
                go.transform.SetParent(transform, false);
                go.transform.localPosition = WorldOf(sq);
                var sr = go.AddComponent<SpriteRenderer>();
                bool light = (sq.File + sq.Rank) % 2 == 1;
                sr.sprite = light ? Catalog?.LightSquare : Catalog?.DarkSquare;
                sr.color = light ? new Color(0.86f, 0.86f, 0.86f) : new Color(0.22f, 0.22f, 0.22f);
                sr.sortingOrder = 0;
                _squares[sq] = go;
            }
        }

        private void SpawnHighlight(Square s, string tag, Color color)
        {
            var go = new GameObject($"Hi_{tag}_{s.ToAlgebraic()}");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = WorldOf(s);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = Catalog?.HighlightSquare;
            sr.color = color;
            sr.sortingOrder = 1;
            var key = s;
            if (_highlights.TryGetValue(key, out var existing) && existing.name.Contains(tag))
                Destroy(existing);
            _highlights[key] = go;
        }

        private void ClearHighlights(string tag)
        {
            var toRemove = new List<Square>();
            foreach (var kv in _highlights)
            {
                if (kv.Value != null && kv.Value.name.Contains($"Hi_{tag}_"))
                {
                    Destroy(kv.Value);
                    toRemove.Add(kv.Key);
                }
            }
            foreach (var k in toRemove) _highlights.Remove(k);
        }
    }
}
