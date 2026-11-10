using UnityEngine;
using Gunnode;

namespace Caijie
{
    // when my level won
    // the screens themselves are run by GameManager, which every level shares.
    public class LevelManager_Caijie : MonoBehaviour
    {
        public static LevelManager_Caijie Instance { get; private set; }

        // PlayerInteractor and UVFlashlight check this. it just asks GameManager
        public static bool IsPlaying => GameManager.IsPlaying;

        private void Awake()
        {
            // only one allowed. if there's already one, remove this copy
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Two LevelManager_Caijie in the scene - destroying the extra one.", this);
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        // when the scene unloads, clear Instance so nothing talks to a destroyed object
        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        // the portal door calls this
        public void CompleteLevel()
        {
            GameManager.CompleteLevel();
        }

        // my level has no lose rule, but ContextMenu adds this to the component's menu,
        // so the Game Over screen can still be tested in Play mode
        [ContextMenu("Game Over (test)")]
        public void GameOver()
        {
            GameManager.Lose();
        }
    }
}