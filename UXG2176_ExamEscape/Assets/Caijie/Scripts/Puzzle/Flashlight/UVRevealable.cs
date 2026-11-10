using System.Collections.Generic;
using UnityEngine;

namespace Caijie
{
    // swaps material between Grey Base and specific colour material

    // this script will be used on the number clues and the buttons.
    // number clues: grey until the UV light hits them, change back if the light leaves
    // buttons: tick alwaysShowColour, so they show their colour all the time without the UV light.
    public class UVRevealable : MonoBehaviour
    {
        // this will be the share list for all revealable object
        // flashlight will be able to loop over instead of searching the whole scene every frame
        // readonly means the list itself cannot be swapped out, but can add and remove item
        public static readonly List<UVRevealable> Active = new List<UVRevealable>();

        // the mesh to swap (button, or the number model)
        [SerializeField] private Renderer[] targetRenderers;

        // one of the colour materials
        // keep it empty on number clues, will be randomly assigned (random each loop)
        [SerializeField] private Material revealedMaterial;

        // the target point that the beam from torch has to reach
        // if its empty, will take the middle of the mesh
        [SerializeField] private Transform revealPoint;

        // tick on the buttons as they always show their colour without light is on them
        // leave it off on the number clues, they will stay grey until the beam hits them
        [SerializeField] private bool alwaysShowColour;

        // set by UVFlashlight every frame
        public bool IsLit { get; set; }

        // set when a button is pressed correctly, so it stays coloured.
        // buttons already have alwaysShowColour, this is just a backup in case it's unticked
        public bool KeepRevealed { get; set; }

        // true while the colour material is on the mesh
        public bool IsRevealed { get; private set; }

        public Vector3 RevealPoint
        {
            get
            {
                if (revealPoint != null) return revealPoint.position;

                // no point set: aim at the middle of the visible mesh, not the model's pivot.
                // a model's pivot can sit at its corner, or inside the wall it's placed on
                if (targetRenderers.Length > 0 && targetRenderers[0] != null)
                    return targetRenderers[0].bounds.center;

                return transform.position;
            }
        }

        // the puzzle reads this to give the matching number clue the same material
        public Material RevealedMaterial => revealedMaterial;

        // for buttons that no need flashlight to reveal color
        public bool AlwaysShowColour => alwaysShowColour;

        private Material hiddenMaterial;   // Grey Base, read from the mesh at the start
        private bool blinkOff;             // the wrong-answer blink forces grey for a moment

        private void Awake()
        {
            // remember the original material that the mesh starts with (Grey Base)
            if (targetRenderers.Length > 0)
                hiddenMaterial = targetRenderers[0].sharedMaterial;
        }

        // join the list when enabled, leave it when disabled or destroyed
        private void OnEnable() => Active.Add(this);
        private void OnDisable() => Active.Remove(this);

        private void Update()
        {
            // show the colour while the beam is on it, once it's been pressed correctly,
            // or all the time for buttons, but not while the puzzle is blinking it off
            bool show = (IsLit || KeepRevealed || alwaysShowColour) && !blinkOff;

            // only swap when it actually changes
            if (show != IsRevealed)
                Swap(show);
        }

        // used by the puzzle to give a number clue its colour material
        public void SetRevealedMaterial(Material material)
        {
            revealedMaterial = material;

            // even if its already showing, swap again so the new colour appears straight away
            if (IsRevealed)
                Swap(true);
        }

        // puzzle feedback: true to force it grey for one moment (the wrong-answer blink)
        public void SetBlinkOff(bool off)
        {
            blinkOff = off;
        }

        // the buttons only light up once the torch is picked up
        public void SetAlwaysShowColour(bool on)
        {
            alwaysShowColour = on;
        }

        // puts either the colour material or Grey Base on every mesh
        private void Swap(bool show)
        {
            IsRevealed = show;

            Material chosen = hiddenMaterial;
            if (show && revealedMaterial != null)
                chosen = revealedMaterial;

            foreach (Renderer r in targetRenderers)
                r.sharedMaterial = chosen;
        }
    }
}