using System.Collections;
using UnityEngine;

namespace Caijie
{
    // the "F | Toggle flashlight" image. same look as the E prompt.
    // it shows the moment the torch is picked up, and fades out the first time F turns it on.
    // it's on this level's canvas, separate from the shared HUD in GameSystems.
    public class FlashlightTip : MonoBehaviour
    {
        [SerializeField] private CanvasGroup tipGroup;   // on the tip image

        // true once the player has turned the torch on, so the tip never comes back
        private bool tipDone;
        public static bool IsShowing { get; private set; }

        private void Awake()
        {
            // hidden until the torch is picked up
            tipGroup.alpha = 0f;
            IsShowing = false;
        }

        // listen to the torch's static events
        private void OnEnable()
        {
            UVFlashlight.Equipped += HandleEquipped;
            UVFlashlight.Toggled += HandleToggled;
        }

        private void OnDisable()
        {
            UVFlashlight.Equipped -= HandleEquipped;
            UVFlashlight.Toggled -= HandleToggled;
            IsShowing = false;
        }

        // the torch was just picked up, and it's still off
        private void HandleEquipped()
        {
            tipGroup.alpha = 1f;
            IsShowing = true;
        }

        private void HandleToggled(bool on)
        {
            // the first time it's turned on, the player has learned the key, so hide the tip
            if (on && !tipDone)
            {
                tipDone = true;
                StartCoroutine(FadeTip());
            }
        }

        // fade the tip out over half a second
        private IEnumerator FadeTip()
        {
            for (float a = 1f; a > 0f; a -= Time.deltaTime * 2f)
            {
                tipGroup.alpha = a;
                yield return null;
            }

            tipGroup.alpha = 0f;
            IsShowing = false;
        }
    }
}