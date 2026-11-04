using UnityEngine;

/// <summary>
/// Add to any object (desk, whiteboard, window...) to show a title and description on the HUD
/// while the player aims at it. Needs a normal (non-trigger) collider.
/// </summary>
public class Inspectable : MonoBehaviour, IInteractable
{
    [SerializeField] private string title = "Object";
    [SerializeField, TextArea(2, 4)] private string description;

    public string GetPrompt(PlayerInteractor interactor) =>
        string.IsNullOrEmpty(description) ? $"<b>{title}</b>" : $"<b>{title}</b>\n{description}";

    public bool CanInteract(PlayerInteractor interactor) => false;   // hover only

    public void Interact(PlayerInteractor interactor) { }
}
