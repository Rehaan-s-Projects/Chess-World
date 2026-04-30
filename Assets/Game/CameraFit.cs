using UnityEngine;

namespace ChessWorld.Game
{
    [RequireComponent(typeof(Camera))]
    public sealed class CameraFit : MonoBehaviour
    {
        public float BoardSize = 8f;
        public float MarginSquares = 1.5f;

        private void Start()
        {
            var cam = GetComponent<Camera>();
            cam.orthographic = true;
            float needed = (BoardSize + MarginSquares * 2f) / 2f;
            float aspect = (float)Screen.width / Screen.height;
            cam.orthographicSize = Mathf.Max(needed, needed / aspect);
        }
    }
}
