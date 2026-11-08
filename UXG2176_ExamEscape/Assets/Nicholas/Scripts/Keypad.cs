using UnityEngine;
using UnityEngine.Events;

public class Keypad : MonoBehaviour
{
    [SerializeField] private string code = "1459";
    public UnityEvent onUnlocked;

    [SerializeField] private string requiredKeyId = "door_keycard";
    public bool CanUse(Inventory inv) => string.IsNullOrEmpty(requiredKeyId) || inv.Has(requiredKeyId);

    public bool IsUnlocked { get; private set; }

    public bool Check(string attempt)
    {
        if (IsUnlocked)
        {
            return true;
        }
        if (attempt != code)
        {
            return false;
        }

        IsUnlocked = true;
        onUnlocked.Invoke();
        return true;
    }
}