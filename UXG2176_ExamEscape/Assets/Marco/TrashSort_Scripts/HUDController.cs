using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>Display only. Listens to events and updates text, never changes game state.</summary>
public class HUDController : MonoBehaviour
{
    [Header("Sources")]
    [SerializeField] private LevelManager_Marco gameManager;
    [SerializeField] private GameTimer timer;
    [SerializeField] private SortingPuzzle puzzle;
    [SerializeField] private PlayerInteractor interactor;

    [Header("Text")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private TMP_Text heldItemText;
    [SerializeField] private TMP_Text feedbackText;

    [Header("Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject levelCompletePanel;

    [Header("Timer colours")]
    [SerializeField] private float lowTimeThreshold = 30f;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color lowTimeColor = Color.red;

    private int lastShownSecond = -1;
    private Coroutine feedbackRoutine;

    private void OnEnable()
    {
        timer.OnTimeChanged += UpdateTimer;
        puzzle.OnSequenceGenerated += RefreshProgress;
        puzzle.OnProgressChanged += RefreshProgress;
        puzzle.OnSubmitted += ShowFeedback;
        interactor.OnPromptChanged += UpdatePrompt;
        interactor.OnHeldItemChanged += UpdateHeldItem;
        gameManager.OnGameOver += ShowGameOver;
        gameManager.OnLevelComplete += ShowLevelComplete;

        gameOverPanel.SetActive(false);
        levelCompletePanel.SetActive(false);
        feedbackText.text = string.Empty;
        promptText.text = string.Empty;
        UpdateHeldItem(null);
    }

    private void OnDisable()
    {
        timer.OnTimeChanged -= UpdateTimer;
        puzzle.OnSequenceGenerated -= RefreshProgress;
        puzzle.OnProgressChanged -= RefreshProgress;
        puzzle.OnSubmitted -= ShowFeedback;
        interactor.OnPromptChanged -= UpdatePrompt;
        interactor.OnHeldItemChanged -= UpdateHeldItem;
        gameManager.OnGameOver -= ShowGameOver;
        gameManager.OnLevelComplete -= ShowLevelComplete;
    }

    private void UpdateTimer(float remaining)
    {
        int seconds = Mathf.CeilToInt(remaining);
        if (seconds == lastShownSecond) return;   // only rebuild the string once per second
        lastShownSecond = seconds;

        timerText.text = $"{seconds / 60:00}:{seconds % 60:00}";
        timerText.color = remaining <= lowTimeThreshold ? lowTimeColor : normalColor;
    }

    private void RefreshProgress()
    {
        progressText.text = $"Sorted {puzzle.Progress}/{puzzle.Sequence.Count}";
    }

    private void UpdatePrompt(string prompt)
    {
        if (promptText != null) promptText.text = prompt;   // may be destroyed during scene unload
    }

    private void UpdateHeldItem(TrashItem item)
    {
        heldItemText.text = item != null
            ? $"Holding: {item.Data.displayName}\n[G] Drop    [Left Click] Throw"
            : "Hands empty";
    }

    private void ShowFeedback(SubmitResult result)
    {
        string message = result switch
        {
            SubmitResult.Correct     => "<color=#55FF55>Correct!</color>",
            SubmitResult.NotSortable => "<color=#FF5555>Useless rubbish! Time penalty</color>",
            SubmitResult.WrongBin    => "<color=#FF5555>Wrong bin! Time penalty</color>",
            SubmitResult.WrongOrder  => "<color=#FF5555>Wrong order! Time penalty</color>",
            _ => string.Empty
        };

        if (feedbackRoutine != null) StopCoroutine(feedbackRoutine);
        feedbackRoutine = StartCoroutine(FlashFeedback(message));
    }

    private IEnumerator FlashFeedback(string message)
    {
        feedbackText.text = message;
        // Real-time wait so it still works if timeScale changes.
        yield return new WaitForSecondsRealtime(2f);
        feedbackText.text = string.Empty;
    }

    private void ShowGameOver() => gameOverPanel.SetActive(true);
    private void ShowLevelComplete() => levelCompletePanel.SetActive(true);
}
