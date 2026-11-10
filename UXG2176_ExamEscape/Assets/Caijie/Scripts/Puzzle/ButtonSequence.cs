using System;
using UnityEngine;

namespace Caijie
{
    // goes on the root of each of the 9 coloured wall buttons.
    // pressing E on it fires Pressed, and SequencePuzzle checks if it was the right one.
    // the colour comes from UVRevealable, the movement from ButtonPressAnimation on the cap.
    [RequireComponent(typeof(UVRevealable))]
    public class ButtonSequence : MonoBehaviour, IInteractable
    {
        // SequencePuzzle subscribes to this
        public event Action<ButtonSequence> Pressed;

        // drag in the mesh that has ButtonPressAnimation on it (the cap).
        // can be left empty, the button still works, it just won't move
        [SerializeField] private ButtonPressAnimation pressAnimation;

        [SerializeField] private Sprite prompt;   // the "E | Press button" image

        public UVRevealable Revealable { get; private set; }
        public bool Locked { get; set; }

        // from IInteractable
        public Sprite Prompt => prompt;
        public bool CanInteract => !Locked && Revealable.IsRevealed;   // not while locked, or while blinking grey

        private void Awake()
        {
            Revealable = GetComponent<UVRevealable>();
        }

        public void Interact(PlayerInteractor interactor)
        {
            // quick click: in and back out
            if (pressAnimation != null)
                pressAnimation.Click();

            // tell the puzzle. if it was correct, the puzzle holds the button down straight after.
            Pressed?.Invoke(this);
        }

        // the puzzle calls this: true = stay pushed in (correct), false = pop back out (after a mistake)
        public void SetHeldDown(bool held)
        {
            if (pressAnimation != null)
                pressAnimation.SetHeldDown(held);
        }
    }
}