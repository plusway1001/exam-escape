using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Caijie
{
    // a message pill in the middle of the screen that stays for a bit and then fades out.
    // light red for a wrong press, light green when the sequence is complete.
    // wired up in the Inspector: SequencePuzzle's onWrongPress and onSolved call it with the text.
    public class MessagePill : MonoBehaviour
    {
        // turn off blocks raycasts on this CanvasGroup so the pill never steals clicks
        [SerializeField] private CanvasGroup group;
        [SerializeField] private Image background;
        [SerializeField] private TMP_Text messageText;

        [SerializeField] private Color warningColour = new Color(1f, 0.8f, 0.8f);   // light red
        [SerializeField] private Color successColour = new Color(0.8f, 1f, 0.8f);   // light green

        [SerializeField] private float holdTime = 1.8f;   // fully visible for this long
        [SerializeField] private float fadeTime = 0.4f;

        private Coroutine routine;

        private void Awake() => group.alpha = 0f;

        // onWrongPress calls this
        public void ShowWarning(string message)
        {
            Show(message, warningColour);
        }

        // onSolved calls this
        public void ShowSuccess(string message)
        {
            Show(message, successColour);
        }

        private void Show(string message, Color colour)
        {
            messageText.text = message;
            background.color = colour;

            // if a message is already showing, stop it and start again,
            // so the new one gets the full time on screen
            if (routine != null)
                StopCoroutine(routine);

            routine = StartCoroutine(Play());
        }

        private IEnumerator Play()
        {
            group.alpha = 1f;
            yield return new WaitForSeconds(holdTime);

            // fade from 1 to 0 over fadeTime seconds
            for (float t = 0f; t < fadeTime; t += Time.deltaTime)
            {
                group.alpha = 1f - t / fadeTime;
                yield return null;
            }

            group.alpha = 0f;
            routine = null;
        }
    }
}