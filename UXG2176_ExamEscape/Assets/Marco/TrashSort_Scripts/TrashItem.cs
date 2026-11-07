using UnityEngine;

/// <summary>
/// A pickable piece of rubbish. Owns its own physics state (held / free / consumed)
/// so no other script has to touch its Rigidbody or colliders.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class TrashItem : MonoBehaviour, IInteractable
{
    [Tooltip("Seconds after being ejected from a bin before it can be submitted again.")]
    [SerializeField] private float resubmitDelay = 1f;

    public TrashItemData Data { get; private set; }
    public bool IsHeld { get; private set; }
    public bool CanBeSubmitted => !IsHeld && !consumed && Time.time >= nextSubmitTime;

    private Rigidbody body;
    private Collider[] colliders;
    private float nextSubmitTime;
    private bool consumed;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
    }

    public void Initialize(TrashItemData data)
    {
        Data = data;
        name = data.displayName;

        if (data.applyTint)
        {
            foreach (var r in GetComponentsInChildren<Renderer>())
                r.material.color = data.tint;
        }
    }

    // ---------- IInteractable ----------
    public string GetPrompt(PlayerInteractor interactor) =>
        interactor.HasItem
            ? "Hands full - drop or throw your item first"
            : $"[E] Pick up {Data.displayName}";

    public bool CanInteract(PlayerInteractor interactor) => !interactor.HasItem && !IsHeld;

    public void Interact(PlayerInteractor interactor) => interactor.PickUp(this);

    // ---------- State changes ----------
    public void AttachTo(Transform holdPoint)
    {
        IsHeld = true;
        body.isKinematic = true;
        SetCollidersEnabled(false);

        transform.SetParent(holdPoint, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Release(Vector3 velocity)
    {
        IsHeld = false;
        transform.SetParent(null, true);
        SetCollidersEnabled(true);
        body.isKinematic = false;
        body.linearVelocity = velocity;   // Unity 6. On older versions use body.velocity
    }

    /// <summary>Pushes the item back out of a bin after a wrong submission.</summary>
    public void Eject(Vector3 position, Vector3 velocity)
    {
        nextSubmitTime = Time.time + resubmitDelay;
        transform.position = position;
        body.linearVelocity = velocity;
        body.angularVelocity = Random.insideUnitSphere * 3f;
    }

    public void Consume()
    {
        consumed = true;
        Destroy(gameObject);
    }

    private void SetCollidersEnabled(bool value)
    {
        foreach (var c in colliders) c.enabled = value;
    }
}
