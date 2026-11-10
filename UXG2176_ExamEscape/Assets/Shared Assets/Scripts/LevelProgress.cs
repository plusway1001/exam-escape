using TMPro;
using UnityEngine;

namespace Gunnode
{
    // the "PUZZLE: 01/05 COMPLETED" text at the top of the screen. it's part of GameScreens,
    // so every level shows it without setting anything up.
    // every level before this one in GameFlow counts as done, so nothing needs saving between scenes.
    public class LevelProgress : MonoBehaviour
    {
        [SerializeField] private TMP_Text progressText;

        private void Start()
        {
            int done = GameManager.LevelIndex;

            // this scene isn't in GameFlow yet: hide instead of showing a wrong number
            if (done < 0)
            {
                gameObject.SetActive(false);
                return;
            }

            // the first level has 0 done, the second has 1 done, and so on.
            // :00 means at least 2 digits, so 1 shows as 01
            progressText.text = $"PUZZLE: {done:00}/{GameManager.LevelCount:00} COMPLETED";
        }
    }
}
