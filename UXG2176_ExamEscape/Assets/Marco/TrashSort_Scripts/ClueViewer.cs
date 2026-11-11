using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Shows the clue panel (the required sorting order). While open, player scripts are switched
/// off and the cursor is freed. The timer keeps running, so reading costs time.
/// Close with E or the Close button (wire the button's OnClick to Close()).
/// Escape is owned by PauseMenu, which calls Close() when this panel is open.
///
/// IMPORTANT: put this script on a different object from the panel (e.g. the Canvas),
/// because the panel gets deactivated.
/// </summary>
public class ClueViewer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelManager_Marco gameManager;
    [SerializeField] private SortingPuzzle puzzle;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text clueText;

    [Tooltip("Player scripts to switch off while reading (BasicMovement, PlayerInteractor).")]
    [SerializeField] private Behaviour[] disableWhileOpen;

    [Header("Input")]
    [SerializeField] private string closeActionName = "Interact";

    public bool IsOpen { get; private set; }

    private readonly StringBuilder builder = new();
    private InputAction closeAction;
    
    private int openedFrame = -1;
    private int closedFrame = -1;

    private void Awake()
    {
        closeAction = InputSystem.actions.FindAction(closeActionName, true);

        panel.SetActive(false);
    }

    private void OnEnable() => gameManager.OnGameOver += HandleGameOver;
    private void OnDisable() => gameManager.OnGameOver -= HandleGameOver;

    private void Update()
    {
        // The shared pause menu (GameScreens) opened on top: put the note away, so Resume drops
        // the player straight back into the game instead of a frozen note with a locked mouse.
        if (IsOpen && !Gunnode.GameManager.IsPlaying)
        {
            Close();
            return;
        }

        // Skip the frame we opened on, otherwise the same E press would close it instantly.
        if (!IsOpen || Time.frameCount == openedFrame) return;

        if (closeAction.WasPressedThisFrame()) Close();
        
    }

    public void Open()
    {
        // closedFrame check stops the closing E press from immediately re-opening the note.
        if (IsOpen || gameManager.State != GameState.Playing || Time.frameCount == closedFrame) return;

        IsOpen = true;
        openedFrame = Time.frameCount;
        clueText.text = BuildClueText();
        panel.SetActive(true);
        SetPlayerControl(false);
    }

    /// <summary>Also hook this to the Close button's OnClick.</summary>
    public void Close()
    {
        if (!IsOpen) return;

        IsOpen = false;
        closedFrame = Time.frameCount;
        panel.SetActive(false);

        // If the game ended while reading, LevelManager_Marco already owns the player/cursor state.
        if (gameManager.State == GameState.Playing) SetPlayerControl(true);
    }

    private void HandleGameOver()
    {
        IsOpen = false;
        panel.SetActive(false);
    }

    private void SetPlayerControl(bool enabled)
    {
        foreach (var behaviour in disableWhileOpen) behaviour.enabled = enabled;

        // While the shared pause / end screens are up, GameManager owns the cursor.
        if (!Gunnode.GameManager.IsPlaying) return;

        Cursor.lockState = enabled ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !enabled;
    }

    private string BuildClueText()
    {
        builder.Clear();
        builder.AppendLine("Sort the rubbish in this order:\n");

        for (int i = 0; i < puzzle.Sequence.Count; i++)
        {
            string label = $"{i + 1}. {puzzle.Sequence[i]}";

            if (i < puzzle.Progress) builder.Append("<color=#55FF55><s>").Append(label).AppendLine("</s></color>"); // done
            else if (i == puzzle.Progress) builder.Append("<b><color=#FFD84D>").Append(label).AppendLine("</color></b>"); // next
            else builder.AppendLine(label);
        }

        return builder.ToString();
    }
}