using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Caijie
{
    // can pick up flashlight and switch light on and off.
    // todo: add the beam that reveals hidden item 
    public class UVFlashlight : MonoBehaviour
    {
        // the flashlight HUD listens to these
        public static event Action Equipped;
        public static event Action<bool> Toggled;    // true = turned on

        // the spot Light child object
        [SerializeField] private Light uvLight;

        private InputAction toggleAction;
        private bool equipped;
        private bool isOn;

        public bool IsOn => isOn;

        private void Awake()
        {
            // the torch lying on the desk starts switched off
            uvLight.enabled = false;
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
            if (!equipped || !LevelManager.IsPlaying) return;

            // F flips the light
            if (toggleAction != null && toggleAction.WasPressedThisFrame())
                SetOn(!isOn); //to flip the bool
        }

        // turns the light on or off
        private void SetOn(bool on)
        {
            isOn = on;
            uvLight.enabled = on;
            Toggled?.Invoke(on);
        }
    }
}