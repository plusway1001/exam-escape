using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Escape key owner. Priority on each press:
///   1) clue panel open        -> close the clue
///   2) instructions open      -> back to the pause menu
///   3) paused                 -> resume
///   4) playing                -> pause
/// Pausing sets timeScale to 0, so the game timer stops too.
/// Put this on the Canvas (NOT on the pause panel, which gets deactivated).
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelManager_Marco levelManager;
    [SerializeField] private ClueViewer clueViewer;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject instructionsPanel;

    [Tooltip("Player scripts to switch off while paused (BasicMovement, PlayerInteractor).")]
    [SerializeField] private Behaviour[] disableWhilePaused;

    [Header("Settings")]
    [Tooltip("Escape in the default Input Actions asset is the UI 'Cancel' action.")]
    [SerializeField] private string pauseActionName = "Cancel";
    [Tooltip("Must be added to File > Build Profiles > Scene List.")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string nextStageSceneName = "Stage2";

    public bool IsPaused { get; private set; }

    private InputAction pauseAction;

    private void Awake()
    {
        pauseAction = InputSystem.actions.FindAction(pauseActionName, true);
        pausePanel.SetActive(false);
        instructionsPanel.SetActive(false);
    }

    private void Update()
    {
        if (!pauseAction.WasPressedThisFrame()) return;
        if (levelManager.State != GameState.Playing) return;   // ignore after win / game over

        if (clueViewer.IsOpen)                    clueViewer.Close();
        else if (!IsPaused)                       Pause();
        else if (instructionsPanel.activeSelf)    CloseInstructions();
        else                                      Resume();
    }

    // ---------- Hook these to the buttons' OnClick ----------
    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        instructionsPanel.SetActive(false);
        SetPlayerControl(true);
    }

    public void OpenInstructions()
    {
        pausePanel.SetActive(false);
        instructionsPanel.SetActive(true);
    }

    public void CloseInstructions()
    {
        instructionsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void Restart()
    {
        IsPaused = false;
        levelManager.Restart();   // resets timeScale and reloads the scene
    }

    public void ReturnToMainMenu()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void NextStage()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(nextStageSceneName);
    }

    // ---------- Internals ----------
    private void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
        instructionsPanel.SetActive(false);
        SetPlayerControl(false);
    }

    private void SetPlayerControl(bool enabled)
    {
        foreach (var behaviour in disableWhilePaused) behaviour.enabled = enabled;

        Cursor.lockState = enabled ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !enabled;
    }
}
