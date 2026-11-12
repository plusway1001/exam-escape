using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Caijie
{
    // can pick up flashlight and switch light on and off.
    // while on, every frame checks which UVRevealables are inside the light cone,
    // and tells each one whether its lit
    public class UVFlashlight : MonoBehaviour
    {
        // the flashlight HUD listens to these
        public static event Action Equipped;
        public static event Action<bool> Toggled;    // true = turned on

        // the spot Light child object
        [SerializeField] private Light uvLight;

        // only the inner part of the cone reveals things
        // 0.8 = inner 80%
        [SerializeField, Range(0.1f, 1f)] private float beamCore = 0.8f;

        // what can block the beam, like walls and furniture
        [SerializeField] private LayerMask occluderMask = ~0;

        private InputAction toggleAction;
        private bool equipped;
        private bool isOn;
        private float cosHalfAngle;

        public bool IsOn => isOn;

        private void Awake()
        {
            // the torch lying on the desk will be switched off in default
            uvLight.enabled = false;

            // read the cone size from the Light itself, so can just change the spot light angle
            // store the cosine of half the angle now, so the beam check in each frame is just one dot product
            // Mathf.Deg2Rad converts degrees to radians, Mathf.Cos only works in radians
            // The dot product of two direction vectors (beam forward and direction to the clue) equals to the cosine of the angle between them
            cosHalfAngle = Mathf.Cos(uvLight.spotAngle * 0.5f * beamCore * Mathf.Deg2Rad);
        }

        // FlashlightPickup calls this through its onPickedUp event
        public void Equip()
        {
            equipped = true;

            // press F to toggle, its alr set in InputSystem
            toggleAction = InputSystem.actions.FindAction("Flashlight");
            Equipped?.Invoke();
        }

        private void Update()
        {
            // not picked up yet, or the game is paused
            if (!equipped || !LevelManager_Caijie.IsPlaying) return;

            // F flips the light
            if (toggleAction != null && toggleAction.WasPressedThisFrame())
                SetOn(!isOn); //toggle to flip the bool

            if (isOn)
                UpdateBeam();
        }

        // turns the light on or off
        private void SetOn(bool on)
        {
            isOn = on;
            uvLight.enabled = on;

            // turning off: everything the beam was touching is no longer lit
            if (!on)
                ClearBeam();

            Toggled?.Invoke(on);
        }

        // goes through every revealable and sets IsLit to true or false
        private void UpdateBeam()
        {
            Vector3 origin = uvLight.transform.position;
            Vector3 forward = uvLight.transform.forward;
            float range = uvLight.range;

            var targets = UVRevealable.Active;
            for (int i = 0; i < targets.Count; i++)
            {
                UVRevealable t = targets[i];

                // buttons always show their colour, skip beam checks on them
                if (t.AlwaysShowColour) continue;

                t.IsLit = IsInBeam(t.RevealPoint, t.transform, origin, forward, range);
            }
        }

        // three checks, cheapest first, so most objects get thrown out early
        private bool IsInBeam(Vector3 point, Transform target, Vector3 origin, Vector3 forward, float range)
        {
            Vector3 toTarget = point - origin;
            float sqrDist = toTarget.sqrMagnitude;

            // 1. is it too far away?
            if (sqrDist > range * range) return false;

            float dist = Mathf.Sqrt(sqrDist);
            if (dist < 0.001f) return true;    // basically touching the torch
            Vector3 dir = toTarget / dist;     // direction with vector of length 1 (basically normlised)

            // 2. outside the cone?
            // for two length-1 vectors, Dot gives the cosine of the angle between them
            // 1 = same direction, 0 = perpendicular, -1 = opposite direction
            // if dot < cosHalfAngle, it means outside the cone
            if (Vector3.Dot(forward, dir) < cosHalfAngle) return false;

            // 3. is it blocked by a wall?
            // more expensve Raycast check
            // Physics.Raycast fires an invisible line from origin in direction dir, but only for dist metres
            // it will stops at the target and cant be blocked by things behind it
            // occluderMask limits it to layers that can block light
            // QueryTriggerInteraction.Ignore makes it skip trigger colliders, so invisible zones dont block the beam
            // if the ray hits something, out RaycastHit hit tells you what
            // the ray stops 10 cm short, so a number stuck flat against a wall isn't blocked by that same wall
            float checkDist = Mathf.Max(0f, dist - 0.1f);
            if (Physics.Raycast(origin, dir, out RaycastHit hit, checkDist, occluderMask, QueryTriggerInteraction.Ignore))
            {
                // if something was hit, fine only if it's the target itself (or a part of it)
                // collider hit might be a child of target instead of the target itself
                return hit.transform.IsChildOf(target);
            }

            // nothing in the way
            return true;
        }

        private void ClearBeam()
        {
            foreach (UVRevealable r in UVRevealable.Active)
                r.IsLit = false;
        }
    }
}