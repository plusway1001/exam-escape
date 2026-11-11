using UnityEngine;
using UnityEngine.InputSystem;

namespace Gunnode
{
    // on the PauseMenu object in the GameScreens prefab. it stays switched on so it can hear Esc,
    // and the pause panel under it is what shows and hides.
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private UIPanel panel;            // the box with Resume and Return to Main Menu
        [SerializeField] private ConfirmDialog confirm;    // the "EXIT LEVEL?" box

        private InputAction pauseAction;
        private bool confirmOpen;

        private void Awake()
        {
            // tell GameManager which panel to show when the game pauses
            GameManager.RegisterPause(panel);

            // Esc is bound to the "Pause" action in the Input Actions asset
            pauseAction = InputSystem.actions.FindAction("Pause");
            if (pauseAction == null)
                Debug.LogWarning("No 'Pause' input action found, so Esc won't work.", this);
        }

        private void Update()
        {
            if (pauseAction == null || !pauseAction.WasPressedThisFrame()) return;

            // Esc closes the confirm box first, then unpauses
            if (confirmOpen)
                CloseConfirm();
            else if (GameManager.IsPaused)
                GameManager.Resume();
            else
                GameManager.Pause();   // does nothing on the Win or Game Over screen
        }

        // resumeBtn
        public void OnResume()
        {
            GameManager.Resume();
        }

        // quitBtn. leaving loses this level's progress, so ask first
        public void OnMainMenu()
        {
            confirmOpen = true;
            panel.SetInteractable(false);   // the pause buttons behind it can't be clicked
            confirm.Setup(GameManager.QuitToMenu, CloseConfirm);
            confirm.Show();
        }

        // No on the confirm box, or Esc while it's open
        private void CloseConfirm()
        {
            confirmOpen = false;
            confirm.Hide();
            panel.SetInteractable(true);
        }
    }
}
