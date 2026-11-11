using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Playing, Won, Lost }

/// <summary>
/// The only script that knows the rules of the whole level. It wires the other systems together:
/// wrong submission -> time penalty, puzzle solved -> stop timer + unlock door, timer 0 -> game over.
/// </summary>
public class LevelManager_Marco : MonoBehaviour
{
    [Header("Systems")]
    [SerializeField] private GameTimer timer;
    [SerializeField] private SortingPuzzle puzzle;
    [SerializeField] private TrashSpawner spawner;
    [SerializeField] private ExitDoor exitDoor;

    [Header("Rules")]
    [SerializeField] private float penaltySeconds = 15f;
    [Tooltip("Scene to load when the door is used. Leave empty to just show Level Complete.")]
    [SerializeField] private string nextSceneName;

    [Header("Player")]
    [Tooltip("Player scripts to switch off when the game ends (BasicMovement, PlayerInteractor...).")]
    [SerializeField] private Behaviour[] disableOnEnd;

    public GameState State { get; private set; } = GameState.Playing;

    public event Action OnGameOver;
    public event Action OnLevelComplete;

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        puzzle.OnSubmitted += HandleSubmitted;
        puzzle.OnSolved += HandleSolved;
        timer.OnExpired += HandleTimeExpired;
        exitDoor.OnUsed += HandleDoorUsed;

        puzzle.Generate();
        spawner.Spawn(puzzle.Sequence);
        timer.Begin();
    }

    private void OnDestroy()
    {
        puzzle.OnSubmitted -= HandleSubmitted;
        puzzle.OnSolved -= HandleSolved;
        timer.OnExpired -= HandleTimeExpired;
        exitDoor.OnUsed -= HandleDoorUsed;
    }

    private void HandleSubmitted(SubmitResult result)
    {
        if (State != GameState.Playing || result == SubmitResult.Correct) return;
        timer.Subtract(penaltySeconds);
    }

    private void HandleSolved()
    {
        if (State != GameState.Playing) return;
        timer.Stop();
        exitDoor.Unlock();
    }

    private void HandleTimeExpired() => EndGame(false);

    private void HandleDoorUsed()
    {
        if (State != GameState.Playing) return;

        if (!string.IsNullOrEmpty(nextSceneName))
            //SceneManager.LoadScene(nextSceneName);
            Gunnode.GameManager.CompleteLevel();
        else
            EndGame(true);
    }

    private void EndGame(bool won)
    {
        if (State != GameState.Playing) return;

        State = won ? GameState.Won : GameState.Lost;
        timer.Stop();

        foreach (var behaviour in disableOnEnd) behaviour.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (won)
        {
            OnLevelComplete?.Invoke();
        }
        else
        {
            Time.timeScale = 0f;
            Gunnode.GameManager.Lose();
            OnGameOver?.Invoke();
        }
    }

    /// <summary>Hook to the Restart button's OnClick.</summary>
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
