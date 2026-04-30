using System;
using UnityEngine;
using ChessWorld.Core;

namespace ChessWorld.Game
{
    public sealed class InputHandler : MonoBehaviour
    {
        public BoardView Board;
        public Camera MainCamera;
        public bool Enabled = true;

        public event Action<Square> SquareTapped;

        private void Update()
        {
            if (!Enabled) return;
            bool tap = false;
            Vector3 screen = Vector3.zero;

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                tap = true;
                screen = Input.GetTouch(0).position;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                tap = true;
                screen = Input.mousePosition;
            }

            if (!tap) return;
            var world = MainCamera.ScreenToWorldPoint(new Vector3(screen.x, screen.y,
                -MainCamera.transform.position.z));
            if (Board.TryWorldToSquare(world, out var sq))
                SquareTapped?.Invoke(sq);
        }
    }
}
