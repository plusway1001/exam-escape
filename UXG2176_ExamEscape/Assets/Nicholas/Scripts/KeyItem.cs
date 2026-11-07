using UnityEngine;

public class KeyItem : MonoBehaviour
{
    public string keyId;
    [SerializeField] private string displayName;

    public string DisplayName => string.IsNullOrEmpty(displayName) ? name : displayName;
}