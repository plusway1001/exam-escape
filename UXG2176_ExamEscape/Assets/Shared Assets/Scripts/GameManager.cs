using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gunnode
{
    // moves the game between scenes: menu -> level 1 -> level 2 -> ... -> menu.
    // it's static, so any script (or a teammate's menu) can call it without an object in the scene.
    // the level order comes from the GameFlow asset in a Resources folder.
    public static partial class GameManager
    {
        private static GameFlow flow;

        // loads the GameFlow asset the first time it's needed, then keeps it
        private static GameFlow Flow
        {
            get
            {
                if (flow == null)
                {
                    flow = Resources.Load<GameFlow>("GameFlow");
                    if (flow == null)
                        Debug.LogError("GameManager: no GameFlow asset in a Resources folder.");
                }
                return flow;
            }
        }

        // where the current scene is in GameFlow: 0 = first level, -1 = not in the list
        public static int LevelIndex
        {
            get
            {
                if (Flow == null) return -1;
                return Flow.IndexOf(SceneManager.GetActiveScene().name);
            }
        }

        public static int LevelCount
        {
            get
            {
                if (Flow == null) return 0;
                return Flow.Count;
            }
        }

        // the last level. a scene that isn't listed counts as last too,
        // so testing a level on its own still ends on the Win screen
        public static bool IsFinalLevel => LevelIndex < 0 || LevelIndex == LevelCount - 1;

        // the menu's Start button
        public static void StartGame()
        {
            if (LevelCount == 0)
            {
                Debug.LogError("GameManager: GameFlow has no level scenes.");
                return;
            }

            Load(Flow.levelScenes[0]);
        }

        public static void LoadNextLevel()
        {
            if (IsFinalLevel)
            {
                QuitToMenu();
                return;
            }

            Load(Flow.levelScenes[LevelIndex + 1]);
        }

        // reloads the scene you're in
        public static void RestartLevel()
        {
            Load(SceneManager.GetActiveScene().name);
        }

        public static void QuitToMenu()
        {
            if (Flow != null)
                Load(Flow.menuScene);
        }

        // the menu's Exit button
        public static void QuitGame()
        {
    #if UNITY_EDITOR
            // Application.Quit does nothing in the Editor, so stop Play mode instead
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif
        }

        // every scene change goes through here, so a fade can be added in this one place later
        private static void Load(string sceneName)
        {
            // catch typos and scenes missing from the build list with a clear message
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError($"GameManager: scene '{sceneName}' can't be loaded. Is it in File > Build Profiles > Scene List?");
                return;
            }

            // a paused or finished level has timeScale 0, and the next scene would start frozen
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }
    }
}
