using UnityEngine;
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
}
