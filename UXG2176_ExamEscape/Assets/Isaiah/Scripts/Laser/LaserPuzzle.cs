using System;
using UnityEngine;
using UnityEngine.Events;

namespace Isaiah
{
    public class LaserPuzzle : MonoBehaviour
    {
        [SerializeField] private LaserReceiver[] receivers;

        [SerializeField] private RotatableMirror[] mirrorsToLock;

        [Header("Events")]
        public UnityEvent onSolved;
        public UnityEvent<int, int> onProgressChanged;

        public event Action<int, int> ProgressChanged;
        public event Action Solved;

        public bool IsSolved { get; private set; }
        public int LitCount { get; private set; }
        public int Total => receivers.Length;

        private void OnEnable()
        {
            foreach (LaserReceiver r in receivers)
                r.PoweredChanged += HandleReceiverChanged;
        }

        private void OnDisable()
        {
            foreach (LaserReceiver r in receivers)
                if (r != null) r.PoweredChanged -= HandleReceiverChanged;
        }

        private void Start() => Recount();

        private void HandleReceiverChanged(LaserReceiver receiver, bool powered) => Recount();

        private void Recount()
        {
            if (IsSolved) return;

            LitCount = 0;
            foreach (LaserReceiver r in receivers)
                if (r.IsPowered) LitCount++;

            ProgressChanged?.Invoke(LitCount, Total);
            onProgressChanged?.Invoke(LitCount, Total);

            if (LitCount == Total && Total > 0)
                Solve();
        }

        private void Solve()
        {
            IsSolved = true;

            foreach (RotatableMirror m in mirrorsToLock)
                if (m != null) m.Lock();

            Solved?.Invoke();
            onSolved?.Invoke();
        }
    }
}
