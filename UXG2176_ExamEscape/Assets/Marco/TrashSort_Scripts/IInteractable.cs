/// <summary>
/// Anything the player can aim at and press E on.
/// PlayerInteractor only knows this interface, so new interactables need no changes to it.
/// </summary>
public interface IInteractable
{
    /// <summary>Text shown on the HUD while the player aims at this object.</summary>
    string GetPrompt(PlayerInteractor interactor);

    bool CanInteract(PlayerInteractor interactor);

    void Interact(PlayerInteractor interactor);
}
