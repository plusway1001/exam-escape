using UnityEngine;

namespace Caijie
{
    // Assets that player can look at and press E on will be using this script
    // This is a reusable script and dont care which object that interacting are
    public interface IInteractable
    {
        // picture for the HUD
        // for example the "E | Pick up flashlight" image
        Sprite Prompt { get; }

        // return false to ignore the player such as: already picked up, still hidden, locked
        bool CanInteract { get; }

        // what happens when E is pressed
        void Interact(PlayerInteractor interactor);
    }
}