using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nicholas
{
    public class Inventory : MonoBehaviour
    {
        private readonly List<ItemData> items = new();

        public ItemData Held => items.Count > 0 ? items[items.Count - 1] : null;
        public event Action<ItemData> HeldChanged;

        public bool Has(ItemData item) => item == null || items.Contains(item);

        public void Add(ItemData item)
        {
            if (item == null || items.Contains(item))
            {
                return;
            }
            items.Add(item);
            HeldChanged?.Invoke(Held);
        }

        public bool Remove(ItemData item)
        {
            if (item == null || !items.Remove(item))
            {
                return false;
            }
            HeldChanged?.Invoke(Held);
            return true;
        }
    }
}