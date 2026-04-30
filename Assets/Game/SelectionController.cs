using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ChessWorld.Core;

namespace ChessWorld.Game
{
    public sealed class SelectionController : MonoBehaviour
    {
        public GameEngine Engine;
        public BoardView Board;
        public Side HumanSide = Side.White;
        public bool Enabled = true;

        public event Action<Move> MoveRequested;

        private Square? _selected;
        private IReadOnlyList<Move> _cachedLegalMoves;

        public void OnSquareTapped(Square sq)
        {
            if (!Enabled) return;
            if (Engine.Board.SideToMove != HumanSide) return;

            if (!_selected.HasValue)
            {
                TrySelect(sq);
                return;
            }

            if (sq == _selected.Value)
            {
                Clear();
                return;
            }

            if (_cachedLegalMoves != null && _cachedLegalMoves.Any(m => m.To == sq))
            {
                var match = _cachedLegalMoves.First(m => m.To == sq);
                MoveRequested?.Invoke(match);
                Clear();
                return;
            }

            var tapped = Engine.Board[sq];
            if (!tapped.IsNone && tapped.Side == HumanSide)
            {
                TrySelect(sq);
                return;
            }

            Clear();
        }

        private void TrySelect(Square sq)
        {
            var p = Engine.Board[sq];
            if (p.IsNone || p.Side != HumanSide) return;
            _selected = sq;
            _cachedLegalMoves = Engine.LegalMovesFrom(sq);
            Board.HighlightSelection(sq);
            Board.HighlightLegalMoves(_cachedLegalMoves.Select(m => m.To));
        }

        private void Clear()
        {
            _selected = null;
            _cachedLegalMoves = null;
            Board.ClearAllHighlights();
        }
    }
}
