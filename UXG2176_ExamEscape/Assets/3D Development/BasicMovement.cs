using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class BasicMovement : MonoBehaviour
{
    public Transform cameraTarget;

    public float mouseSensitivity;
    public float upperLookLimit;
    public float lowerLookLimit;
    public bool canPitch;

    [Header("Movement")]
    public float moveSpeed = 1f;
    public float jumpHeight = 1f;
    public float gravity = -9.81f;

    private float cameraPitch = 0f;
    private float verticalVelocity = 0f;
    private CharacterController controller;
    private InputAction moveAction, pointerAction, jumpAction;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        moveAction = InputSystem.actions.FindAction("Move");
        pointerAction = InputSystem.actions.FindAction("Pointer");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    void Update()
    {
        // --- Horizontal movement ---
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.forward * moveInput.y + transform.right * moveInput.x;
        move = move.normalized * moveSpeed;

        // --- Gravity + Jump ---
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f; // keeps the player stuck to the ground
        }

        if (jumpAction.WasPressedThisFrame() && controller.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        // --- Look ---
        Vector2 lookInput = pointerAction.ReadValue<Vector2>();

        float characterYaw = lookInput.x * mouseSensitivity;
        transform.Rotate(Vector3.up * characterYaw);

        if (canPitch)
        {
            cameraPitch -= lookInput.y * mouseSensitivity;
            cameraPitch = Mathf.Clamp(cameraPitch, upperLookLimit, lowerLookLimit);
        }
        else
        {
            cameraPitch = 0.0f;
        }
        cameraTarget.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }
}

/*using UnityEngine;
using UnityEngine.InputSystem;

public class BasicMovement : MonoBehaviour
{
    public Transform cameraTarget;

    public float mouseSensitivity;
    public float upperLookLimit;
    public float lowerLookLimit;
    public bool canPitch;

    private float cameraPitch = 0f;
    private InputAction moveAction, pointerAction;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        pointerAction = InputSystem.actions.FindAction("Pointer");
    }

    void Update()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.forward * moveInput.y + transform.right * moveInput.x;
        transform.position += move.normalized * Time.deltaTime;

        Vector2 lookInput = pointerAction.ReadValue<Vector2>();

        float characterYaw = lookInput.x * mouseSensitivity;
        transform.Rotate(Vector3.up * characterYaw);

        if(canPitch)
        {
            cameraPitch -= lookInput.y * mouseSensitivity;
            cameraPitch = Mathf.Clamp(cameraPitch, upperLookLimit, lowerLookLimit);

            cameraTarget.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }
        else
        {
            cameraPitch = 0.0f;
        }
        cameraTarget.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }
}*/
