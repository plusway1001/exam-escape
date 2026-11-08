using System.Text;
using TMPro;
using UnityEngine;

namespace Nicholas
{
    public class HoverPrompt : MonoBehaviour
    {
        [SerializeField] private PlayerSelect select;
        [SerializeField] private Inventory inventory;
        [SerializeField] private TMP_Text label;

        private readonly StringBuilder sb = new();

        private void Update()
        {
            sb.Clear();
            GameConfig cfg = select.Config;
            Swappable swappable = select.Swappable;
            Interactable interactable = select.Interactable;

            InteractState state = InteractState.Hidden;
            string text = null;
            if (interactable != null)
            {
                state = interactable.GetState(inventory, out text);
            }
            bool showInteract = state != InteractState.Hidden;

            if (showInteract)
            {
                sb.AppendLine(interactable.DisplayName);
            }
            else if (swappable != null)
            {
                sb.AppendLine(swappable.DisplayName);
            }

            if (swappable != null)
            {
                Action(cfg.swapPromptColor, cfg.swapPrompt);
            }

            if (state == InteractState.Available)
            {
                Action(cfg.availablePromptColor, $"[{cfg.interactKey}] {text}");
            }
            else if (state == InteractState.Blocked)
            {
                Action(cfg.blockedPromptColor, text);
            }

            label.text = sb.ToString();
        }

        private void Action(Color color, string text)
        {
            sb.AppendLine(string.Format(select.Config.actionFormat, ColorUtility.ToHtmlStringRGB(color), text));
        }
    }
}