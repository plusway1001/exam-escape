using UnityEngine;

namespace Nicholas
{
    public class PlayerSelect : MonoBehaviour
    {
        [SerializeField] private GameConfig config;
        public GameConfig Config => config;

        public Swappable Swappable { get; private set; }
        public Interactable Interactable { get; private set; }

        private void Update()
        {
            Swappable = null;
            Interactable = null;
            if (InputBlocker.InputBlocked)
            {
                return;
            }

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit swapHit, config.aimRange, config.blockerMask))
            {
                Swappable = swapHit.collider.GetComponentInParent<Swappable>();
            }

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit useHit, config.interactRange, config.interactMask))
            {
                Interactable = useHit.collider.GetComponentInParent<Interactable>();
            }
        }
    }
}