using System;
using UnityEngine;

namespace Caijie
{
    // goes on the root of each of the 9 coloured wall buttons.
    // pressing E to press, and SequencePuzzle checks if it was the right one.
    // the colour comes from UVRevealable, the movement from ButtonPressAnimation on the cap.
    //By doinig RequireComponent, it forces to add one if its missing
    [RequireComponent(typeof(UVRevealable))]
    public class ButtonSequence : MonoBehaviour, IInteractable
    {
        // SequencePuzzle subscribes to this
        // Mainly let the puzzle know which button is pressed
        public event Action<ButtonSequence> Pressed;

        // drag in the mesh that has ButtonPressAnimation on it (the cap).
        // can be left empty, the button still works, it just won't move
        [SerializeField] private ButtonPressAnimation pressAnimation;

        [SerializeField] private Sprite prompt;   // the "E  Press button" image

        // puzzle will use this to keep the button coloured or blink it when errored
        public UVRevealable Revealable { get; private set; }
        // set to true by puzzle when the button was pressed correctly
        // to make sure button dont get pressed double
        public bool Locked { get; set; }

        // from IInteractable
        public Sprite Prompt => prompt;
        // not while locked, or while blinking grey
        public bool CanInteract => !Locked && Revealable.IsRevealed;   

        private void Awake()
        {
            Revealable = GetComponent<UVRevealable>();
        }

        public void Interact(PlayerInteractor interactor)
        {
            // quick click: in and back out
            if (pressAnimation != null)
                pressAnimation.Click();

            AudioManager.Instance.PlaySfxAt(AudioManager.Instance.sounds.btnPress, transform.position);

            // if it was the correct button in order, the puzzle holds the button down straight after.
            Pressed?.Invoke(this);
        }

        // true = stay pushed in (correct), false = pop back out (after a mistake)
        public void SetHeldDown(bool held)
        {
            if (pressAnimation != null)
                pressAnimation.SetHeldDown(held);
        }
    }
}