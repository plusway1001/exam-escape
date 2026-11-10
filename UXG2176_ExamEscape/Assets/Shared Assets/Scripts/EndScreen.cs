using UnityEngine;

namespace Gunnode
{
    // on the GameOver and Win objects in the GameScreens prefab. like PauseMenu,
    // the object stays switched on and the panel under it is what shows.
    // tick Is Win Screen on the Win one.
    public class EndScreen : MonoBehaviour
    {
        [SerializeField] private bool isWinScreen;
        [SerializeField] private UIPanel panel;

        private void Awake()
        {
            // tell GameManager which panel to show for Lose() or the last CompleteLevel()
            GameManager.RegisterEnd(panel, isWinScreen);
        }

        // restartBtn (Game Over only)
        public void OnRestart()
        {
            GameManager.RestartLevel();
        }

        // Return to Main Menu on both
        public void OnMainMenu()
        {
            GameManager.QuitToMenu();
        }
    }
}
