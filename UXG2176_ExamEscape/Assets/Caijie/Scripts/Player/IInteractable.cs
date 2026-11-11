namespace Caijie
{
    // Assets that player can look at and press E on will be using this script
    // This is a reusable script and dont care which object that interacting are
    public interface IInteractable
    {
        // text for the HUD
        // for example "Pick up UV flashlight"
        string Prompt { get; }

        // return false to ignore the player such as: already picked up, still hidden, locked
        bool CanInteract { get; }

        // what happens when E is pressed
        void Interact(PlayerInteractor interactor);
    }
}