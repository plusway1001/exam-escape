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
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpHeight = 1f;
    public float gravity = -9.81f;

    private float cameraPitch = 0f;
    private float verticalVelocity = 0f;
    private CharacterController controller;
    private InputAction moveAction, pointerAction, jumpAction, sprintAction;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        moveAction = InputSystem.actions.FindAction("Move");
        pointerAction = InputSystem.actions.FindAction("Pointer");
        jumpAction = InputSystem.actions.FindAction("Jump");
        sprintAction = InputSystem.actions.FindAction("Sprint");
    }

    void Update()
    {
        // --- Horizontal movement ---
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.forward * moveInput.y + transform.right * moveInput.x;

        bool isRunning = sprintAction.IsPressed();   // true while Shift is held
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        move = move.normalized * currentSpeed;

        // --- Gravity + Jump ---
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
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