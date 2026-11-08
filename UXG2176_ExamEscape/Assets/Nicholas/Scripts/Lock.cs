using UnityEngine;
using UnityEngine.Events;

namespace Nicholas
{
    public class Lock : Interactable
    {
        [SerializeField] private ItemData requiredItem;
        [SerializeField] private string unlockText = "Unlock";
        [SerializeField] private string lockedText = "Locked";
        [SerializeField] private bool consumeItem = true;

        public UnityEvent onUnlocked;

        private bool unlocked;

        public override InteractState GetState(Inventory inv, out string text)
        {
            if (unlocked)
            {
                text = null; return InteractState.Hidden;
            }
            bool hasItem = inv.Has(requiredItem);
            text = hasItem ? unlockText : lockedText;
            return hasItem ? InteractState.Available : InteractState.Blocked;
        }

        public override void Interact(Inventory inv)
        {
            if (unlocked || !inv.Has(requiredItem))
            {
                return;
            }
            unlocked = true;
            if (consumeItem)
            {
                inv.Remove(requiredItem);
            }
            onUnlocked.Invoke();
        }
    }
}