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

        // null means hide
        private void HandleChanged(Sprite image)
        {
            if (image == null)
            {
                promptImage.gameObject.SetActive(false);
                return;
            }

            promptImage.sprite = image;
            promptImage.gameObject.SetActive(true);
        }
    }
}
