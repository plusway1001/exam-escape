using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Caijie
{
    // the brain of the puzzle.
    // it holds the correct order, listens to every button, checks each press,
    // and fires onSolved once all of them are pressed in order without a mistake.
    public class SequencePuzzle : MonoBehaviour
    {
        [Header("Buttons")]
        [Tooltip("Correct order = order in this list (after shuffling, if enabled).")]
        [SerializeField] private ButtonSequence[] sequence;

        // extra buttons that are always wrong (optional)
        [SerializeField] private ButtonSequence[] decoys;

        // the number models in the room. clues[0] is "1", clues[1] is "2", and so on
        [SerializeField] private UVRevealable[] clues;

        // same room, different answer every loop
        [SerializeField] private bool shuffleEachLoop = true;

        // a wrong press also scrambles the order
        [SerializeField] private bool reshuffleOnMistake = true;

        [Header("Feedback")]
        [SerializeField] private int wrongBlinks = 3;
        [SerializeField] private float blinkInterval = 0.12f;
        [SerializeField] private float wrongTimePenalty = 10f;    // seconds taken off the timer

        [Header("Events")]
        public UnityEvent onCorrectPress;
        public UnityEvent onWrongPress;   // the warning popup (A8) and a buzz sound
        public UnityEvent onSolved;       // PuzzleDoor.Unlock goes here

        private int step;       // how many buttons are done so far
        private bool busy;      // true while the wrong-answer blink is playing
        private bool solved;

        private void Awake()
        {
            // new random order every time the scene loads (every loop)
            if (shuffleEachLoop)
                Shuffle(sequence);
        }

        // start listening to every button. each += in OnEnable has a matching -= in OnDisable.
        private void OnEnable()
        {
            foreach (ButtonSequence b in sequence)
                b.Pressed += HandlePress;
            foreach (ButtonSequence b in decoys)
                b.Pressed += HandlePress;
        }

        private void OnDisable()
        {
            foreach (ButtonSequence b in sequence)
                b.Pressed -= HandlePress;
            foreach (ButtonSequence b in decoys)
                b.Pressed -= HandlePress;
        }

        private void Start()
        {
            // done in Start, not Awake, because the buttons read their own colours in their Awake.
            // by the time any Start runs, every Awake has finished.
            AssignClueColours();    // written in task 4
            ReportProgress();
        }

        // runs every time any button (or decoy) is pressed
        private void HandlePress(ButtonSequence button)
        {
            if (solved || busy) return;

            if (button == sequence[step])
            {
                // correct
                button.Locked = true;                     // can't press it twice
                button.SetHeldDown(true);                 // stays pushed in
                button.Revealable.KeepRevealed = true;    // stays coloured without the UV light

                step++;
                onCorrectPress.Invoke();
                ReportProgress();

                if (step == sequence.Length)
                    Solve();
            }
            else
            {
                // wrong button or a decoy
                StartCoroutine(WrongAnswer(button));
            }
        }

        private IEnumerator WrongAnswer(ButtonSequence wrongButton)
        {
            busy = true;    // ignore any presses while this plays
            onWrongPress.Invoke();

            if (LevelManager.Instance != null)
                LevelManager.Instance.AddTime(-wrongTimePenalty);

            // only blink the buttons the player already found, so the others aren't given away
            List<UVRevealable> found = new List<UVRevealable>();
            found.Add(wrongButton.Revealable);
            for (int i = 0; i < step; i++)
                found.Add(sequence[i].Revealable);

            // make the wait once and reuse it, instead of creating a new one every blink
            WaitForSeconds wait = new WaitForSeconds(blinkInterval);

            // flash them between grey and their colour a few times
            for (int b = 0; b < wrongBlinks; b++)
            {
                foreach (UVRevealable r in found) r.SetBlinkOff(true);    // grey
                yield return wait;
                foreach (UVRevealable r in found) r.SetBlinkOff(false);   // colour again
                yield return wait;
            }

            // undo all the correct presses
            for (int i = 0; i < step; i++)
            {
                sequence[i].Locked = false;
                sequence[i].SetHeldDown(false);                // pops back out
                sequence[i].Revealable.KeepRevealed = false;   // back to Grey Base (unless the beam is on it)
            }

            // shuffle after the reset, because the reset above needs the old order
            // to know which buttons were pressed. the numbers now show new colours.
            if (reshuffleOnMistake)
            {
                Shuffle(sequence);
                AssignClueColours();
            }

            step = 0;
            ReportProgress();
            busy = false;
        }

        private void Solve()
        {
            solved = true;

            // decoys can't be pressed any more either
            foreach (ButtonSequence d in decoys)
                d.Locked = true;

            onSolved.Invoke();
        }

        // sends "done / total" to the LevelManager, which passes it on to the HUD (Buttons 3 / 9)
        private void ReportProgress()
        {
            if (LevelManager.Instance != null)
                LevelManager.Instance.ReportProgress(step, sequence.Length);
        }

        // number "1" gets the first button's colour material, "2" the second's, and so on
        private void AssignClueColours()
        {
            for (int i = 0; i < clues.Length && i < sequence.Length; i++)
                clues[i].SetRevealedMaterial(sequence[i].Revealable.RevealedMaterial);
        }

        // fisher-yates shuffle: walk backwards through the array and swap each item
        // with a random one at or before it. every order is equally likely.
        private static void Shuffle<T>(T[] items)
        {
            for (int i = items.Length - 1; i > 0; i--)
            {
                // with ints, Random.Range never returns the max, so +1 to include i itself
                int j = Random.Range(0, i + 1);

                // swap items i and j
                (items[i], items[j]) = (items[j], items[i]);
            }
        }

        // runs in the Editor whenever a value changes in the Inspector.
        // warns about setup mistakes before you even press Play.
        private void OnValidate()
        {
            if (sequence == null || sequence.Length < 3)
            {
                Debug.LogWarning($"{name}: the brief needs at least 3 interactions - add more buttons.", this);
                return;
            }

            HashSet<ButtonSequence> seen = new HashSet<ButtonSequence>();
            foreach (ButtonSequence b in sequence)
            {
                if (b == null)
                    Debug.LogWarning($"{name}: empty slot in the sequence list.", this);
                else if (!seen.Add(b))
                    Debug.LogWarning($"{name}: '{b.name}' is in the sequence twice.", this);
            }

            if (clues == null || clues.Length != sequence.Length)
                Debug.LogWarning($"{name}: needs exactly one number clue per ButtonSequence ({sequence.Length}).", this);
        }
    }
}
