using UnityEngine;
using UnityEngine.Events;

namespace Nicholas
{
    public class PickupItem : Interactable
    {
        [SerializeField] private ItemData item;
        [SerializeField] private string actionText = "Pick up";
        public override string DisplayName => item != null ? item.displayName : base.DisplayName;

        public UnityEvent onPickedUp;
        public override InteractState GetState(Inventory inv, out string text)
        {
            text = actionText;
            return InteractState.Available;
        }
        public override void Interact(Inventory inv)
        {
            inv.Add(item);
            onPickedUp.Invoke();
            Destroy(gameObject);
        }
    }
}