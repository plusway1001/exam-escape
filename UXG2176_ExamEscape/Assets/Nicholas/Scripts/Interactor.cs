using UnityEngine;
using UnityEngine.InputSystem;

namespace Nicholas
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private PlayerSelect select;
        [SerializeField] private Inventory inventory;

        private void Update()
        {
            if (InputBlocker.InputBlocked)
            {
                return;
            }
            if (Keyboard.current[select.Config.interactKey].wasPressedThisFrame && select.Interactable != null)
            {
                select.Interactable.Interact(inventory);
            }
        }
    }
}