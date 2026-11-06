using UnityEngine;
using UnityEngine.Events;

namespace Caijie
{
    // implements IInteractable, so PlayerInteractor can pick it up with no extra code
    // when picked up, it attaches itself to the player's hand and fires onPickedUp.
    public class FlashlightPickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private string prompt = "Pick up UV flashlight";

        // where it sits in the hand
        [SerializeField] private Vector3 heldPosition = Vector3.zero;
        [SerializeField] private Vector3 heldRotation = Vector3.zero;

        // drag the flashlight in inspector and choose UVFlashlight.Equip
        [Tooltip("Hook UVFlashlight.Equip here")]
        public UnityEvent onPickedUp;

        private bool pickedUp;

        //from IInteractable
        public string Prompt => prompt;
        public bool CanInteract => !pickedUp;   // only once

        public void Interact(PlayerInteractor interactor)
        {
            pickedUp = true;

            // switch off physics and colliders first,
            // or the torch would drop out of the hand or bump into the player
            if (TryGetComponent(out Rigidbody rb))
                rb.isKinematic = true;

            foreach (Collider col in GetComponentsInChildren<Collider>())
                col.enabled = false;

            // make it a child of the hand so it moves with the player
            transform.SetParent(interactor.HoldSocket, false);
            transform.localPosition = heldPosition;
            // turns normal X/Y/Z angles (angle in the inspector) into a Quaternion, the type Unity uses for rotations
            transform.localRotation = Quaternion.Euler(heldRotation);

            onPickedUp.Invoke();
        }
    }
}