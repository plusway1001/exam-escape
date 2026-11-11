using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Raycasts from the camera centre to find an IInteractable within range, and manages the
/// single carried item (pick up / drop / throw). Put this on the Player object.
/// </summary>
public class PlayerInteractor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [Tooltip("Empty child of the camera, e.g. local position (0, -0.25, 0.9).")]
    [SerializeField] private Transform holdPoint;

    [Header("Interaction")]
    [SerializeField] private float interactRange = 3f;
    [Tooltip("Exclude the Player layer so the ray never hits yourself.")]
    [SerializeField] private LayerMask interactMask = ~0;

    [Header("Throwing")]
    [SerializeField] private float throwSpeed = 8f;

    [Header("Input action names (Input Actions asset)")]
    [SerializeField] private string interactActionName = "Interact"; // E
    [SerializeField] private string dropActionName = "Drop";         // G
    [Tooltip("Used automatically if the Input Actions asset has no Drop action.")]
    [SerializeField] private string fallbackDropBinding = "<Keyboard>/g";
    [SerializeField] private string throwActionName = "Attack";      // Left mouse

    public TrashItem HeldItem { get; private set; }
    public bool HasItem => HeldItem != null;

    public event Action<string> OnPromptChanged;
    public event Action<TrashItem> OnHeldItemChanged;

    private InputAction interactAction, dropAction, throwAction;
    private IInteractable current;
    private bool promptDirty = true;
    private bool createdDropAction;

    private void Awake()
    {
        if (cameraTransform == null) cameraTransform = Camera.main.transform;

        interactAction = InputSystem.actions.FindAction(interactActionName, true);

        // Prefer the asset's Drop action, but don't depend on it: create one in code if missing.
        dropAction = InputSystem.actions.FindAction(dropActionName, false);
        if (dropAction == null)
        {
            //Debug.LogWarning($"PlayerInteractor: no '{dropActionName}' action found, using {fallbackDropBinding}.", this);
            dropAction = new InputAction(dropActionName, InputActionType.Button, fallbackDropBinding);
            dropAction.Enable();
            createdDropAction = true;
        }

        throwAction = InputSystem.actions.FindAction(throwActionName, true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        if (createdDropAction) dropAction.Dispose();
    }

    private void OnDisable()
    {
        current = null;
        promptDirty = true;
        OnPromptChanged?.Invoke(string.Empty);
    }

    private void Update()
    {
        UpdateTarget();

        if (current != null && interactAction.WasPressedThisFrame() && current.CanInteract(this))
            current.Interact(this);

        if (!HasItem) return;

        if (dropAction.WasPressedThisFrame())
            Release(Vector3.zero);
        else if (throwAction.WasPressedThisFrame())
            Release(cameraTransform.forward * throwSpeed);
    }

    private void UpdateTarget()
    {
        IInteractable target = null;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit,
                interactRange, interactMask, QueryTriggerInteraction.Ignore))
        {
            target = hit.collider.GetComponentInParent<IInteractable>();
        }

        // Only rebuild the prompt string when something actually changed.
        if (target == current && !promptDirty) return;

        current = target;
        promptDirty = false;
        OnPromptChanged?.Invoke(current != null ? current.GetPrompt(this) : string.Empty);
    }

    public void PickUp(TrashItem item)
    {
        if (HasItem) return;

        HeldItem = item;
        item.AttachTo(holdPoint);
        promptDirty = true;
        OnHeldItemChanged?.Invoke(HeldItem);
    }

    private void Release(Vector3 velocity)
    {
        HeldItem.Release(velocity);
        HeldItem = null;
        promptDirty = true;
        OnHeldItemChanged?.Invoke(null);
    }
}


/*using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Raycasts from the camera centre to find an IInteractable within range, and manages the
/// single carried item (pick up / drop / throw). Put this on the Player object.
/// </summary>
public class PlayerInteractor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [Tooltip("Empty child of the camera, e.g. local position (0, -0.25, 0.9).")]
    [SerializeField] private Transform holdPoint;

    [Header("Interaction")]
    [SerializeField] private float interactRange = 3f;
    [Tooltip("Exclude the Player layer so the ray never hits yourself.")]
    [SerializeField] private LayerMask interactMask = ~0;

    [Header("Throwing")]
    [SerializeField] private float throwSpeed = 8f;

    [Header("Input action names (Input Actions asset)")]
    [SerializeField] private string interactActionName = "Interact"; // E
    [SerializeField] private string dropActionName = "Drop";         // G (you must add this one)
    [SerializeField] private string throwActionName = "Attack";      // Left mouse

    public TrashItem HeldItem { get; private set; }
    public bool HasItem => HeldItem != null;

    public event Action<string> OnPromptChanged;
    public event Action<TrashItem> OnHeldItemChanged;

    private InputAction interactAction, dropAction, throwAction;
    private IInteractable current;
    private bool promptDirty = true;

    private void Awake()
    {
        if (cameraTransform == null) cameraTransform = Camera.main.transform;

        interactAction = InputSystem.actions.FindAction(interactActionName, true);
        dropAction = InputSystem.actions.FindAction(dropActionName, true);
        throwAction = InputSystem.actions.FindAction(throwActionName, true);
    }

    private void OnDisable()
    {
        current = null;
        promptDirty = true;
        OnPromptChanged?.Invoke(string.Empty);
    }

    private void Update()
    {
        UpdateTarget();

        if (current != null && interactAction.WasPressedThisFrame() && current.CanInteract(this))
            current.Interact(this);

        if (!HasItem) return;

        if (dropAction.WasPressedThisFrame())
            Release(Vector3.zero);
        else if (throwAction.WasPressedThisFrame())
            Release(cameraTransform.forward * throwSpeed);
    }

    private void UpdateTarget()
    {
        IInteractable target = null;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit,
                interactRange, interactMask, QueryTriggerInteraction.Ignore))
        {
            target = hit.collider.GetComponentInParent<IInteractable>();
        }

        // Only rebuild the prompt string when something actually changed.
        if (target == current && !promptDirty) return;

        current = target;
        promptDirty = false;
        OnPromptChanged?.Invoke(current != null ? current.GetPrompt(this) : string.Empty);
    }

    public void PickUp(TrashItem item)
    {
        if (HasItem) return;

        HeldItem = item;
        item.AttachTo(holdPoint);
        promptDirty = true;
        OnHeldItemChanged?.Invoke(HeldItem);
    }

    private void Release(Vector3 velocity)
    {
        HeldItem.Release(velocity);
        HeldItem = null;
        promptDirty = true;
        OnHeldItemChanged?.Invoke(null);
    }
}*/
