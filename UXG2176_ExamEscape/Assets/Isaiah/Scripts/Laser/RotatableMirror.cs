using UnityEngine;

namespace Isaiah
{
    public class RotatableMirror : MonoBehaviour, Caijie.IInteractable
    {
        [Header("Rotation")]
        [SerializeField] private Transform pivot;
        [SerializeField] private float stepDegrees = 45f;
        [SerializeField] private float turnSpeed = 360f;

        [Header("Feedback")]
        [SerializeField] private string prompt = "[E] Rotate mirror";
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip turnClip;

        private Quaternion startRotation;
        private float currentYaw;
        private float targetYaw;
        private bool locked;

        public string Prompt => prompt;
        public bool CanInteract => !locked;

        private void Awake()
        {
            if (pivot == null) pivot = transform;

            startRotation = pivot.localRotation;
        }

        public void Interact(Caijie.PlayerInteractor interactor)
        {
            targetYaw = Mathf.Repeat(targetYaw + stepDegrees, 360f);

            if (audioSource != null && turnClip != null)
                audioSource.PlayOneShot(turnClip);
        }

        private void Update()
        {
            currentYaw = Mathf.MoveTowardsAngle(currentYaw, targetYaw, turnSpeed * Time.deltaTime);

            pivot.localRotation = startRotation * Quaternion.Euler(0f, currentYaw, 0f);
        }

        public void Lock() => locked = true;
    }
}
