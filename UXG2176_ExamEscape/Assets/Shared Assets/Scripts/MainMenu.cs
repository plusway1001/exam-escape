using UnityEngine;
using UnityEngine.UI;

namespace Gunnode
{
    // goes on the menu's Canvas. startBtn and quitBtn call these from OnClick.
    // OnClick can't pick a static method, so these pass the click on to GameManager.
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private ConfirmDialog confirm;   // the "exit game?" box
        [SerializeField] private Button[] menuButtons;    // Start and Exit, switched off while the box is open

        private void Start()
        {
            // coming back from a level, the cursor could still be locked and hidden
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // startBtn
        public void OnStart()
        {
            GameManager.StartGame();
        }

        // quitBtn. closing the game can't be undone, so ask first
        public void OnQuit()
        {
            if (confirm == null)
            {
                Debug.LogWarning("MainMenu: no confirm box set, quitting without asking.", this);
                GameManager.QuitGame();
                return;
            }

            SetButtons(false);
            confirm.Setup(GameManager.QuitGame, CloseConfirm);
            confirm.Show();
        }

        // No on the box
        private void CloseConfirm()
        {
            confirm.Hide();
            SetButtons(true);
        }

        private void SetButtons(bool on)
        {
            foreach (Button b in menuButtons)
            {
                if (b != null)
                    b.interactable = on;
            }
        }
    }
}
