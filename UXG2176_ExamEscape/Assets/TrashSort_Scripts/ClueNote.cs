using UnityEngine;

/// <summary>
/// A piece of paper in the world. Aim at it and press E to open the clue panel.
/// Needs a normal (non-trigger) collider so the interaction ray can hit it.
/// </summary>
public class ClueNote : MonoBehaviour, IInteractable
{
    [SerializeField] private ClueViewer viewer;
    [SerializeField] private string prompt = "[E] Read note";

    [Header("Audio (optional)")]
    [SerializeField] private AudioClip readClip;

    public string GetPrompt(PlayerInteractor interactor) => prompt;

    public bool CanInteract(PlayerInteractor interactor) => !viewer.IsOpen;

    public void Interact(PlayerInteractor interactor)
    {
        if (readClip != null) AudioSource.PlayClipAtPoint(readClip, transform.position);
        viewer.Open();
    }
}
