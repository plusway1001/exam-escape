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
    public float crouchSpeed = 1.5f;
    public float jumpHeight = 1f;
    public float gravity = -9.81f;

    [Header("Crouch")]
    public float crouchHeight = 1f;
    public float crouchTransitionSpeed = 10f;

    private float cameraPitch = 0f;
    private float verticalVelocity = 0f;
    private CharacterController controller;
    private InputAction moveAction, pointerAction, jumpAction, sprintAction, crouchAction;

    // Stored standing values
    private float standingHeight;
    private Vector3 standingCenter;
    private float standingCameraY;
    private bool isCrouching;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        moveAction = InputSystem.actions.FindAction("Move");
        pointerAction = InputSystem.actions.FindAction("Pointer");
        jumpAction = InputSystem.actions.FindAction("Jump");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        crouchAction = InputSystem.actions.FindAction("Crouch");

        standingHeight = controller.height;
        standingCenter = controller.center;
        standingCameraY = cameraTarget.localPosition.y;
    }

    void Update()
    {
        if (!Gunnode.GameManager.IsPlaying) return;
        HandleCrouch();
        HandleMovement();
        HandleLook();
    }

    void HandleCrouch()
    {
        bool wantsToCrouch = crouchAction.IsPressed();

        if (wantsToCrouch)
        {
            isCrouching = true;
        }
        else if (isCrouching && CanStandUp())
        {
            isCrouching = false;
        }

        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        float heightDifference = standingHeight - targetHeight;

        // Smoothly change the capsule height, keeping the feet on the ground
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        float currentDifference = standingHeight - controller.height;
        controller.center = new Vector3(standingCenter.x, standingCenter.y - currentDifference / 2f, standingCenter.z);

        // Lower the camera along with the capsule
        Vector3 camPos = cameraTarget.localPosition;
        float targetCamY = standingCameraY - heightDifference;
        camPos.y = Mathf.Lerp(camPos.y, targetCamY, Time.deltaTime * crouchTransitionSpeed);
        cameraTarget.localPosition = camPos;
    }

    bool CanStandUp()
    {
        // Cast upward from the top of the capsule to check for a ceiling
        Bounds b = controller.bounds;
        Vector3 origin = new Vector3(b.center.x, b.max.y - 0.01f, b.center.z);
        float checkDistance = (standingHeight - controller.height) + 0.1f;

        return !Physics.SphereCast(origin, controller.radius * 0.9f, Vector3.up,
            out _, checkDistance, ~0, QueryTriggerInteraction.Ignore);
    }

    void HandleMovement()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.forward * moveInput.y + transform.right * moveInput.x;

        // Crouch overrides sprint
        float currentSpeed = walkSpeed;
        if (isCrouching) currentSpeed = crouchSpeed;
        else if (sprintAction.IsPressed()) currentSpeed = runSpeed;

        move = move.normalized * currentSpeed;

        // Gravity + Jump
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        // Can't jump while crouching
        if (jumpAction.WasPressedThisFrame() && controller.isGrounded && !isCrouching)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    void HandleLook()
    {
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