using UnityEngine;
using UnityEngine.Events;

public class Lock : MonoBehaviour
{
    [SerializeField] private string requiredKeyId;
    public UnityEvent onUnlocked;

    private bool unlocked;

    public bool IsUnlocked => unlocked;
    public string RequiredKeyId => requiredKeyId;

    public bool TryUnlock(Inventory inv)
    {
        if (unlocked || !inv.Has(requiredKeyId))
        {
            return false;
        }
        unlocked = true;
        onUnlocked.Invoke();
        return true;
    }
}