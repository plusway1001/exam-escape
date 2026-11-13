using UnityEngine;
using UnityEngine.UI;

namespace Caijie
{
    // the interact prompt: one Image that shows whatever picture the object hands over,
    // like "E | Pick up flashlight" or "E | Press button".
    // it listens to InteractionPrompt, so any script that calls InteractionPrompt.Show can use it.
    public class InteractPromptUI : MonoBehaviour
    {
        // a child Image, not this object itself,
        // because hiding this object would also stop it listening
        [SerializeField] private Image promptImage;
        // the picture the player is looking at right now, null if nothing.
        // kept even while hidden, so it can pop up once the F tip is gone
        private Sprite current;

        private void OnEnable()
        {
            InteractionPrompt.Changed += HandleChanged;
        }

        private void OnDisable()
        {
            InteractionPrompt.Changed -= HandleChanged;
        }

        private void Start()
        {
            // every picture fits inside the box set in the Inspector, without stretching
            promptImage.preserveAspect = true;
            promptImage.gameObject.SetActive(false);
        }

        // null means hide. only remember the picture here, Update decides if it's shown
        private void HandleChanged(Sprite image)
        {
            current = image;

            if (image != null)
                promptImage.sprite = image;
        }

        private void Update()
        {
            // show only when there's something to interact with and the F tip isn't on screen,
            // so the two prompts never overlap
            bool show = current != null && !FlashlightTip.IsShowing;

            // only touch it when it actually changes
            if (promptImage.gameObject.activeSelf != show)
                promptImage.gameObject.SetActive(show);
        }
    }
}
