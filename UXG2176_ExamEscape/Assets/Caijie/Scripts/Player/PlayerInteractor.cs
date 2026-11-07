using UnityEngine;
using UnityEngine.InputSystem;

namespace Caijie
{
    // attached on Player
    // every frame shoots a ray from the middle of the screen
    // if the ray hits something with IInteractable on it, the prompt shows on the HUD, and pressing E calls Interact on it.
    public class PlayerInteractor : MonoBehaviour
    {
        // leave empty and Start will use Camera.main
        [SerializeField] private Camera viewCamera;

        // empty child object on the player
        // used as the "hand" for picked up items
        [SerializeField] private Transform holdSocket;

        // how far the player can reach, in meters, measured from the body not the camera
        [SerializeField] private float range = 2.5f;

        // which layers the ray can hit
        // ~0 means all of them
        [SerializeField] private LayerMask interactMask = ~0;

        private InputAction interactAction;

        // whatever is under the crosshair right now, or null
        // basically where user looking at
        private IInteractable current;

        // flashlightPickup needs to know where the hand is
        public Transform HoldSocket => holdSocket;

        private void Start()
        {
            // with Cinemachine, the real Camera is Main Camera
            // rmb to tag MainCamera
            if (viewCamera == null)
                viewCamera = Camera.main;

            // "Interact" is the action name inside InputSystem_Actions (bound to E)
            interactAction = InputSystem.actions.FindAction("Interact");
        }

        // if this script is switched off, clear the prompt so it doesn't get stuck on screen
        private void OnDisable() => SetFocus(null);

        private void Update()
        {
            // no interacting while paused or on the win/lose screens
            if (!LevelManager.IsPlaying)
            {
                SetFocus(null);
                return;
            }

            // check what player looking at this frame
            SetFocus(FindTarget());

            // true when looking at something and E was just pressed within the frame
            if (current != null && interactAction.WasPressedThisFrame())
                current.Interact(this);
        }


        private IInteractable FindTarget()
        {
            // a ray from the camera through the middle of the screen, where the crosshair is
            // viewport coordinates go from 0 to 1 across the screen, (0.5, 0.5) means centered of the screen
            Ray ray = viewCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            // Dev tool, to show the reach ray in Scene view
            Debug.DrawRay(ray.origin, ray.direction * range, Color.green);

            // first person, so the ray starts at the player's position
            // nothing hit means nothing to look at
            // "Raycast" returns true/false, "out" hands back what was hit and where
            // the ray is only "range" long, so anything it hits is already within reach.
            // "QueryTriggerInteraction.Ignore", the ray will go straight through trigger colliders
            if (!Physics.Raycast(ray, out RaycastHit hit, range, interactMask, QueryTriggerInteraction.Ignore))
                return null;


            // temporary: prints how far away the thing you're looking at is
            Debug.Log(hit.collider.name + " is " + hit.distance.ToString("0.0") + " m away");

            // collider is often on a child mesh, so look up through the objects to the parents for the script
            IInteractable target = hit.collider.GetComponentInParent<IInteractable>();

            if (target != null && target.CanInteract)
                return target;

            return null;
        }

        // updates which object we're focused on, and the prompt with it
        private void SetFocus(IInteractable target)
        {
            // same thing as last frame, nothing to do
            if (target == current) return;

            current = target;

            if (current != null)
                InteractionPrompt.Show(current.Prompt);
            else
                InteractionPrompt.Hide();
        }
    }
}