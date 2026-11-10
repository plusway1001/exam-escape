using TMPro;
using UnityEngine;

namespace Caijie
{
    // the "BUTTONS PRESSED: 3/9" pill on my level's HUD.
    // it listens to SequencePuzzle, so it goes up on every correct press and back to 0 after a mistake.
    // it stays hidden until the torch is picked up, same as the buttons' colours.
    public class ButtonCounter : MonoBehaviour
    {
        [SerializeField] private SequencePuzzle puzzle;
        [SerializeField] private TMP_Text counterText;
        [SerializeField] private CanvasGroup group;   // the pill and its text together

        private void Awake()
        {
            group.alpha = 0f;
        }

        private void OnEnable()
        {
            puzzle.ProgressChanged += HandleProgress;
            UVFlashlight.Equipped += HandleEquipped;
        }

        private void OnDisable()
        {
            puzzle.ProgressChanged -= HandleProgress;
            UVFlashlight.Equipped -= HandleEquipped;
        }

        private void HandleEquipped()
        {
            group.alpha = 1f;
        }

        private void HandleProgress(int done, int total)
        {
            counterText.text = $"BUTTONS PRESSED: {done}/{total}";
        }
    }
}
