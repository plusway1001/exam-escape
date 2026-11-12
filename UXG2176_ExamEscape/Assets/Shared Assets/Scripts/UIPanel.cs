using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gunnode
{
    // base class for all menu panels (pause, confirm, game over, win).
    // handles showing and hiding with a quick fade that still works while paused,
    // and selects a button when the panel opens so keyboard and gamepad work.
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPanel : MonoBehaviour
    {
        // the button that's highlighted when the panel opens
        [SerializeField] private Selectable firstSelected;
        [SerializeField] private float fadeTime = 0.15f;

        private CanvasGroup group;
        private Coroutine fade;

        // gets the CanvasGroup the first time it's needed.
        // a panel that starts hidden never runs Awake, so we can't rely on Awake for this.
        private CanvasGroup Group
        {
            get
            {
                if (group == null)
                    group = GetComponent<CanvasGroup>();
                return group;
            }
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            SetInteractable(true);
            FadeTo(1f);
        }

        public virtual void Hide()
        {
            SetInteractable(false);
            FadeTo(0f);
        }

        public void SetInteractable(bool value)
        {
            Group.interactable = value;      // buttons can be pressed
            Group.blocksRaycasts = value;    // clicks stop here instead of going through

            if (value && firstSelected != null && EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
        }

        private void FadeTo(float target)
        {
            if (fade != null)
                StopCoroutine(fade);

            // coroutines can't run on a switched-off object, so just set the alpha
            if (!gameObject.activeInHierarchy)
            {
                Group.alpha = target;
                return;
            }

            fade = StartCoroutine(FadeRoutine(target));
        }

        private IEnumerator FadeRoutine(float target)
        {
            float start = Group.alpha;

            // unscaled time so the fade works while the game is paused
            for (float t = 0f; t < fadeTime; t += Time.unscaledDeltaTime)
            {
                Group.alpha = Mathf.Lerp(start, target, t / fadeTime);
                yield return null;
            }

            Group.alpha = target;

            // fully hidden: switch it off so it costs nothing to draw
            if (target == 0f)
                gameObject.SetActive(false);

            fade = null;
        }
    }
}