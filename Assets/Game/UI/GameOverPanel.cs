using UnityEngine;
using UnityEngine.UI;
using ChessWorld.Core;

namespace ChessWorld.Game.UI
{
    public sealed class GameOverPanel : MonoBehaviour
    {
        public GameObject Root;
        public Text Headline;
        public Text Credits;
        public Button RestartButton;

        public System.Action OnRestart;

        private void Awake()
        {
            if (Root != null) Root.SetActive(false);
            if (RestartButton != null)
                RestartButton.onClick.AddListener(() => OnRestart?.Invoke());
            if (Credits != null)
                Credits.text = "Background art by Rehaan Rashid";
        }

        public void Show(GameResult result)
        {
            if (Root != null) Root.SetActive(true);
            if (Headline != null)
            {
                Headline.text = result switch
                {
                    GameResult.WhiteWins => "Humanity survives.",
                    GameResult.BlackWins => "The zombies win.",
                    GameResult.Draw => "A stalemate in the ruins.",
                    _ => ""
                };
            }
        }

        public void Hide()
        {
            if (Root != null) Root.SetActive(false);
        }
    }
}
