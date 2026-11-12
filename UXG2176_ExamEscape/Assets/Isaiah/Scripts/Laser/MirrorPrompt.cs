using UnityEngine;

namespace Isaiah
{
    public class MirrorPromptText : MonoBehaviour
    {
        [SerializeField] private Camera viewCamera;
        [SerializeField] private GameObject mirrorText;
        [SerializeField] private float range = 2.5f;
        [SerializeField] private LayerMask interactMask = ~0;

        private void Start()
        {
            if (viewCamera == null)
                viewCamera = Camera.main;

            if (mirrorText != null)
                mirrorText.SetActive(false);
        }

        private void Update()
        {
            if (mirrorText == null || viewCamera == null) return;

            bool lookingAtMirror = false;

            Ray ray = viewCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, range, interactMask, QueryTriggerInteraction.Ignore))
            {
                RotatableMirror mirror = hit.collider.GetComponentInParent<RotatableMirror>();
                lookingAtMirror = mirror != null && mirror.CanInteract;
            }

            if (mirrorText.activeSelf != lookingAtMirror)
                mirrorText.SetActive(lookingAtMirror);
        }

        private void OnDisable()
        {
            if (mirrorText != null)
                mirrorText.SetActive(false);
        }
    }
}