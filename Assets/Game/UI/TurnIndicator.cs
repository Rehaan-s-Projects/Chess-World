using UnityEngine;
using UnityEngine.UI;
using ChessWorld.Core;

namespace ChessWorld.Game.UI
{
    public sealed class TurnIndicator : MonoBehaviour
    {
        public Text Label;

        public void SetTurn(Side s)
        {
            if (Label == null) return;
            Label.text = s == Side.White ? "Your turn" : "Zombies thinking…";
        }
    }
}
