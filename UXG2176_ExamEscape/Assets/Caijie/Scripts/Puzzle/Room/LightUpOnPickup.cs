using UnityEngine;

namespace Caijie
{
    // keeps a button grey until the player picks up the UV torch, then lights it up.
    // goes on each button root, next to UVRevealable.
    // take it off and the button goes back to being coloured from the start.
    [RequireComponent(typeof(UVRevealable))]
    public class LightUpOnPickup : MonoBehaviour
    {
        private UVRevealable revealable;

        private void Awake()
        {
            revealable = GetComponent<UVRevealable>();

            // start grey, even though Always Show Colour is ticked on the prefab
            revealable.SetAlwaysShowColour(false);
        }

        // Equipped is static, so subscribe through the class name.
        // the -= matters: after a loop restart, a destroyed button would still be in the list
        private void OnEnable()
        {
            UVFlashlight.Equipped += HandleTorchPickedUp;
        }

        private void OnDisable()
        {
            UVFlashlight.Equipped -= HandleTorchPickedUp;
        }

        private void HandleTorchPickedUp()
        {
            revealable.SetAlwaysShowColour(true);
        }
    }
}