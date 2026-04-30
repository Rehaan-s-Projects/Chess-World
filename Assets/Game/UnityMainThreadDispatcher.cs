using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessWorld.Game
{
    public sealed class UnityMainThreadDispatcher : MonoBehaviour
    {
        private static UnityMainThreadDispatcher _instance;
        private readonly Queue<Action> _queue = new Queue<Action>();
        private readonly object _lock = new object();

        public static UnityMainThreadDispatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("UnityMainThreadDispatcher");
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<UnityMainThreadDispatcher>();
                }
                return _instance;
            }
        }

        public void Enqueue(Action action)
        {
            lock (_lock) _queue.Enqueue(action);
        }

        private void Update()
        {
            while (true)
            {
                Action a;
                lock (_lock)
                {
                    if (_queue.Count == 0) break;
                    a = _queue.Dequeue();
                }
                a();
            }
        }
    }
}
