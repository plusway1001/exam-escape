using UnityEngine;
using UnityEngine.Events;

namespace Nicholas
{
    public class SwapTrigger : MonoBehaviour
    {
        [SerializeField] private Swappable a;
        [SerializeField] private Swappable b;
        public UnityEvent onSwapped;

        private bool fired;

        private void OnEnable() => Swappable.Swapped += Handle;
        private void OnDisable() => Swappable.Swapped -= Handle;

        private void Handle(Swappable x, Swappable y)
        {
            if (fired)
            {
                return;
            }
            bool match = (x == a && y == b) || (x == b && y == a);
            if (!match)
            {
                return;
            }

            fired = true;
            onSwapped.Invoke();
        }
    }
}