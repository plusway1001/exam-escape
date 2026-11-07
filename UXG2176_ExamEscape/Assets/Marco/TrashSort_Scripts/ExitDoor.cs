using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Locked until the puzzle is solved. Aim at it within range and press E to use it.
/// Needs a normal (non-trigger) collider so the interaction ray can hit it.
/// </summary>
public class ExitDoor : MonoBehaviour, IInteractable
{
    [Header("Visuals (optional)")]
    [SerializeField] private Renderer indicator;
    [SerializeField] private Color lockedColor = Color.red;
    [SerializeField] private Color unlockedColor = Color.green;

    [Header("Prompts")]
    [SerializeField] private string lockedPrompt = "Locked - sort the rubbish in the right order";
    [SerializeField] private string unlockedPrompt = "[E] Go to next room";

    [Tooltip("Hook up door animation / sound here.")]
    public UnityEvent onUnlocked;

    public bool IsUnlocked { get; private set; }
    public event Action OnUsed;

    private void Start() => RefreshVisual();

    public void Unlock()
    {
        if (IsUnlocked) return;
        IsUnlocked = true;
        RefreshVisual();
        onUnlocked?.Invoke();
    }

    public string GetPrompt(PlayerInteractor interactor) => IsUnlocked ? unlockedPrompt : lockedPrompt;

    public bool CanInteract(PlayerInteractor interactor) => IsUnlocked;

    public void Interact(PlayerInteractor interactor) => OnUsed?.Invoke();

    private void RefreshVisual()
    {
        if (indicator != null) indicator.material.color = IsUnlocked ? unlockedColor : lockedColor;
    }
}
