using UnityEngine;

namespace Gunnode
{
    // the order of every level in the game, first to last. one asset for whole team.
    // to make one: right-click in the Project window > Create > Time Loop > Game Flow
    [CreateAssetMenu(fileName = "GameFlow", menuName = "Time Loop/Game Flow")]
    public class GameFlow : ScriptableObject
    {
        [Tooltip("The main menu scene. Return to Main Menu and the end of the game go here.")]
        public string menuScene = "Main";

        [Tooltip("Level scenes in play order. Names must match File > Build Profiles > Scene List.")]
        public string[] levelScenes = { "Caijie" };

        public int Count => levelScenes.Length;

        // where a scene is in the list: 0 for the first level, 1 for the second, and so on.
        // -1 if the scene isn't in the list (like the menu or a test scene)
        public int IndexOf(string sceneName)
        {
            return System.Array.IndexOf(levelScenes, sceneName);
        }
    }
}