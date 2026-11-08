using UnityEngine;
using UnityEngine.Events;

namespace Nicholas
{
    public class Keypad : Interactable
    {
        [SerializeField] private string code = "1459";
        [SerializeField] private ItemData requiredItem;
        [SerializeField] private string enterText = "Enter code";
        [SerializeField] private string lockedText = "Locked";
        public UnityEvent onUnlocked;

        public bool IsUnlocked { get; private set; }
        public int CodeLength => code.Length;

        [SerializeField] private bool consumeItem = true;

        private Inventory user;

        public bool Check(string attempt)
        {
            if (IsUnlocked)
            {
                return true;
            }
            if (attempt != code)
            {
                return false;
            }
            IsUnlocked = true;
            if (consumeItem && user != null)
            {
                user.Remove(requiredItem);
            }
            onUnlocked.Invoke();
            return true;
        }

        public override InteractState GetState(Inventory inv, out string text)
        {
            if (IsUnlocked)
            {
                text = null;
                return InteractState.Hidden;
            }
            bool ok = inv.Has(requiredItem);
            text = ok ? enterText : lockedText;
            return ok ? InteractState.Available : InteractState.Blocked;
        }

        public override void Interact(Inventory inv)
        {
            if (IsUnlocked || !inv.Has(requiredItem))
            {
                return;
            }
            user = inv;
            KeypadUI.Instance.Open(this);
        }
    }
}