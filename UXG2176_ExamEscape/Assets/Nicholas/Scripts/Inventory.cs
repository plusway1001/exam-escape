using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private readonly HashSet<string> items = new();

    public bool Has(string id) => items.Contains(id);
    public void Add(string id) => items.Add(id);
}