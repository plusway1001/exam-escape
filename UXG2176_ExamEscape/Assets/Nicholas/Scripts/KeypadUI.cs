using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nicholas
{
    public class KeypadUI : MonoBehaviour
    {
        public static KeypadUI Instance { get; private set; }

        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private TMP_Text feedback;

        private Keypad current;

        [SerializeField] private Key closeKey = Key.Escape;
        [SerializeField] private Key submitKey = Key.Enter;

        public bool IsOpen => panel.activeSelf;

        private void Awake()
        {
            Instance = this;
            input.characterValidation = TMP_InputField.CharacterValidation.Digit;
            panel.SetActive(false);
        }

        private void Update()
        {
            if (!IsOpen)
            {
                return;
            }
            if (Keyboard.current[closeKey].wasPressedThisFrame)
            {
                Close();
            }
            if (Keyboard.current[submitKey].wasPressedThisFrame)
            {
                Submit();
            }
        }

        public void Open(Keypad lockToOpen)
        {
            current = lockToOpen;
            input.text = "";
            feedback.text = "";
            panel.SetActive(true);
            InputBlocker.SetInputBlocked(true);
            input.ActivateInputField();
        }

        public void Close()
        {
            panel.SetActive(false);
            current = null;
            InputBlocker.SetInputBlocked(false);
        }

        public void Submit()
        {
            if (current == null) return;

            if (current.Check(input.text))
            {
                Close();
            }
            else
            {
                feedback.text = "<color=#FF6666>Wrong code. Try again.</color>";
                input.text = "";
                input.ActivateInputField();
                AudioManager.Instance.PlaySfx(AudioManager.Instance.sounds.wrong);
            }
        }

        public void Backspace()
        {
            if (input.text.Length > 0)
            {
                input.text = input.text.Substring(0, input.text.Length - 1);
            }
        }
    }
}