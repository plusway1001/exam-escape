using UnityEngine;

namespace Caijie
{
    // turns on the fog from the Lighting window once the torch is picked up.
    // colour, mode and density stay set in the Lighting window,
    // this script only switches it off at the start and back on at pickup.
    public class FogOnPickup : MonoBehaviour
    {
        private void Awake()
        {
            // Fog is ticked in the Lighting window, but the room starts clear
            RenderSettings.fog = false;
        }

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
            RenderSettings.fog = true;
        }
    }
}