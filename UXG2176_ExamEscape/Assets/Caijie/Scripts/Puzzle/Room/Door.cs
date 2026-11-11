using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Caijie
{
    // goes on the Door parent, with the door model and the trigger zone as children.
    // SequencePuzzle's onSolved calls Unlock(): the door turns into a glowing white portal,
    // stops blocking, and its trigger zone switches on. walking into the zone ends the level.
    [RequireComponent(typeof(Rigidbody))]
    public class Door : MonoBehaviour
    {
        // the door model's mesh(es) that turn white
        [SerializeField] private Renderer[] doorRenderers;

        // a white material with emission on, made by you like the button colours
        [SerializeField] private Material portalMaterial;

        // the door model's solid collider(s). switched off so the door stops blocking
        [SerializeField] private Collider[] blockers;

        // the trigger zone child, just in front of the door.
        // switched off until the door opens, so the level can't end before the puzzle is solved
        [SerializeField] private Collider triggerZone;

        // optional: a point light in front of the door, so the glow spills onto the floor and walls
        [SerializeField] private Light portalLight;
        [SerializeField] private float glowTime = 1f;

        // for a sound, particles, and so on
        public UnityEvent onOpened;

        private bool isOpen;
        private float lightTarget;   // how bright the light is set in the Inspector

        // Unity calls Reset when you first add the component.
        // the Rigidbody only has to exist so this script hears the trigger zone, it should never move
        private void Reset()
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }

        private void Awake()
        {
            // remember the light's brightness, then start it dark
            if (portalLight != null)
            {
                lightTarget = portalLight.intensity;
                portalLight.intensity = 0f;
            }

            if (triggerZone != null)
            {
                triggerZone.isTrigger = true;    // in case it was left unticked
                triggerZone.enabled = false;
            }
        }

        public void Unlock()
        {
            // only open once
            if (isOpen) return;
            isOpen = true;

            // turn every material slot on the door white
            foreach (Renderer r in doorRenderers)
            {
                Material[] slots = r.sharedMaterials;
                for (int i = 0; i < slots.Length; i++)
                    slots[i] = portalMaterial;

                r.sharedMaterials = slots;
            }

            // the door stops blocking the player
            foreach (Collider c in blockers)
                c.enabled = false;

            // now stepping in front of the door ends the level
            if (triggerZone != null)
                triggerZone.enabled = true;

            if (portalLight != null)
                StartCoroutine(FadeInLight());

            onOpened.Invoke();
        }

        // the trigger zone is a child collider, but the Rigidbody is on this object,
        // so Unity sends the child's trigger messages here too
        private void OnTriggerEnter(Collider other)
        {
            if (!isOpen) return;

            if (other.CompareTag("Player") && LevelManager_Caijie.Instance != null)
                LevelManager_Caijie.Instance.CompleteLevel();
        }

        // brightens the light from 0 to its Inspector value over glowTime seconds
        private IEnumerator FadeInLight()
        {
            for (float t = 0f; t < 1f; t += Time.deltaTime / glowTime)
            {
                portalLight.intensity = Mathf.Lerp(0f, lightTarget, t);
                yield return null;
            }

            portalLight.intensity = lightTarget;
        }
    }
}