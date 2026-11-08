using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nicholas
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private string emptyText = "None";

        private void OnEnable()
        {
            inventory.HeldChanged += Show;
            Show(inventory.Held);
        }

        private void OnDisable() => inventory.HeldChanged -= Show;

        private void Show(ItemData item)
        {
            itemName.text = item != null ? item.displayName : emptyText;

            bool hasIcon = item != null && item.icon != null;
            icon.sprite = hasIcon ? item.icon : null;
            icon.enabled = hasIcon;
        }
    }
}