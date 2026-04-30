using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChessWorld.Core;

namespace ChessWorld.Game.UI
{
    public sealed class CapturedTray : MonoBehaviour
    {
        public Side DisplaysCapturesOfSide;   // e.g. Black means zombies captured by white
        public Transform IconContainer;
        public SpriteCatalog Catalog;

        private readonly List<GameObject> _icons = new List<GameObject>();

        public void AppendCapture(Piece captured)
        {
            if (captured.Side != DisplaysCapturesOfSide) return;
            if (IconContainer == null || Catalog == null) return;

            var go = new GameObject($"Cap_{captured.Side}_{captured.Type}");
            go.transform.SetParent(IconContainer, false);
            var img = go.AddComponent<Image>();
            img.sprite = Catalog.GetPiece(captured.Side, captured.Type);
            img.preserveAspect = true;
            _icons.Add(go);
        }

        public void Clear()
        {
            foreach (var g in _icons) if (g != null) Destroy(g);
            _icons.Clear();
        }
    }
}
