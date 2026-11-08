using UnityEngine;

namespace Nicholas
{
    public enum InteractState { Hidden, Available, Blocked }

    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] private string displayName;
        public virtual string DisplayName => string.IsNullOrEmpty(displayName) ? name : displayName;
        public abstract InteractState GetState(Inventory inv, out string text);
        public abstract void Interact(Inventory inv);
    }
}