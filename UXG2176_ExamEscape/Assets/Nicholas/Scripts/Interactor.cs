using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private float range = 3f;

    private void Update()
    {
        if (!Keyboard.current.eKey.wasPressedThisFrame)
        {
            return;
        }
        if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, range))
        {
            return;
        }

        if (hit.collider.GetComponentInParent<KeyItem>() is KeyItem key)
        {
            inventory.Add(key.keyId);
            Destroy(key.gameObject);
        }
        else if (hit.collider.GetComponentInParent<Lock>() is Lock l)
        {
            l.TryUnlock(inventory);
        }
        else if (hit.collider.GetComponentInParent<Keypad>() is Keypad cl && !cl.IsUnlocked)
        {
            if (cl.CanUse(inventory))
            {
                KeypadUI.Instance.Open(cl);
            }
        }
    }
}