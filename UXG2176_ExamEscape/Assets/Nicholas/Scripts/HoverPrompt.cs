using System.Text;
using TMPro;
using UnityEngine;

public class HoverPrompt : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Inventory inventory;

    [SerializeField] private float swapRange = 8f;
    [SerializeField] private float interactRange = 3f;

    [SerializeField] private LayerMask blockerMask = ~0;

    private readonly StringBuilder sb = new();

    private void Update()
    {
        sb.Clear();

        Swappable swappable = null;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit swapHit, swapRange, blockerMask, QueryTriggerInteraction.Ignore))
        {
            swappable = swapHit.collider.GetComponentInParent<Swappable>();
        }

        KeyItem key = null;
        Lock lk = null;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactRange, ~0, QueryTriggerInteraction.Ignore))
        {
            key = hit.collider.GetComponentInParent<KeyItem>();
            lk = hit.collider.GetComponentInParent<Lock>();
        }

        if (key != null)
        {
            sb.AppendLine(key.DisplayName);
        }
        else if (swappable != null)
        {
            sb.AppendLine(swappable.DisplayName);
        }

        if (swappable != null)
        {
            sb.AppendLine("<size=70%><color=#55FF55>[Click] Swap</color></size>");
        }

        if (key != null)
        {
            sb.AppendLine("<size=70%>[E] Pick up</size>");
        }
        else if (lk != null && !lk.IsUnlocked)
        {
            sb.AppendLine(inventory.Has(lk.RequiredKeyId)
                ? "<size=70%>[E] Unlock</size>"
                : "<size=70%><color=#FF6666>Locked</color></size>");
        }

        label.text = sb.ToString();
    }
}