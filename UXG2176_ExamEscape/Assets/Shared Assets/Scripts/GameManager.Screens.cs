using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Gunnode
{
    // the second half of GameManager: pause, win and lose.
    // the screens are prefabs in each level. each one tells GameManager it's there in its Awake,
    // so any level can call GameManager.Lose() without a reference to the screen.
    public static partial class GameManager
    {
        public static bool IsPaused { get; private set; }

        // true once the Win or Game Over screen is up, or the next level is loading
        public static bool IsOver { get; private set; }

        // movement, interaction and puzzle scripts check this before doing anything
        public static bool IsPlaying => !IsPaused && !IsOver;

        // the panels the screens hand over. GameManager only needs to show and hide them
        private static UIPanel pausePanel;
        private static UIPanel winPanel;
        private static UIPanel losePanel;

        // runs once when the game starts, before the first scene loads
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            IsPaused = false;
            IsOver = false;

            // -= first, so it can never be added twice
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        // every new scene starts unpaused, not over, and with the mouse locked for playing.
        // the menu frees the mouse again in its own Start
        private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            IsPaused = false;
            IsOver = false;
            Freeze(false);
        }

        // PauseMenu and EndScreen call these from their Awake
        public static void RegisterPause(UIPanel panel)
        {
            pausePanel = panel;
        }

        public static void RegisterEnd(UIPanel panel, bool isWinScreen)
        {
            if (isWinScreen)
                winPanel = panel;
            else
                losePanel = panel;
        }

        public static void Pause()
        {
            if (!IsPlaying) return;

            if (pausePanel == null)
            {
                Debug.LogWarning("GameManager: no pause menu in this scene. Add the GameScreens prefab.");
                return;
            }

            IsPaused = true;
            Freeze(true);
            pausePanel.Show();
        }

        public static void Resume()
        {
            if (!IsPaused) return;

            IsPaused = false;
            Freeze(false);
            pausePanel.Hide();
        }

        // a level calls this at its exit: the next level, or the Win screen after the last one
        public static void CompleteLevel()
        {
            if (!IsPlaying) return;

            if (IsFinalLevel)
            {
                AudioManager.Instance.PlaySfx(AudioManager.Instance.sounds.win);
                ShowEnd(winPanel);
            }
            else
            {
                // blocks a second call (walking into the exit twice) while the next scene loads
                IsOver = true;
                LoadNextLevel();
            }
        }

        // a level calls this when the player loses, whatever losing means in that level
        public static void Lose()
        {
            if (!IsPlaying) return;
            AudioManager.Instance.PlaySfx(AudioManager.Instance.sounds.gameOver);
            ShowEnd(losePanel);
        }

        private static void ShowEnd(UIPanel panel)
        {
            if (panel == null)
            {
                Debug.LogWarning("GameManager: no end screen in this scene. Add the GameScreens prefab.");
                return;
            }

            IsOver = true;
            Freeze(true);
            panel.Show();
        }

        // stops the game and frees the mouse for a menu, or the other way round
        private static void Freeze(bool frozen)
        {
            if (frozen)
            {
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            SetLookInput(!frozen);
        }

        // the player turns the camera with the mouse straight from these actions, and timeScale 0
        // doesn't stop that. a switched-off action reads as zero, so the camera stays still behind a menu.
        // the menus click through the separate UI actions, so they still work
        private static void SetLookInput(bool on)
        {
            if (InputSystem.actions == null) return;

            foreach (string name in new[] { "Pointer", "Look" })
            {
                InputAction action = InputSystem.actions.FindAction(name);
                if (action == null) continue;

                if (on)
                    action.Enable();
                else
                    action.Disable();
            }
        }
    }
}