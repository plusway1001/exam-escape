using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeypadUI : MonoBehaviour
{
    public static KeypadUI Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_InputField input;
    [SerializeField] private TMP_Text feedback;

    [SerializeField] private Behaviour[] disableWhileOpen;

    private Keypad current;

    public bool IsOpen => panel.activeSelf;

    private void Awake()
    {
        Instance = this;
        input.characterValidation = TMP_InputField.CharacterValidation.Digit;
        panel.SetActive(false);
    }

    private void Update()
    {
        if (!IsOpen) return;
        if (Keyboard.current.escapeKey.wasPressedThisFrame) Close();
        if (Keyboard.current.enterKey.wasPressedThisFrame) Submit();
    }

    public void Open(Keypad lockToOpen)
    {
        current = lockToOpen;
        input.text = "";
        feedback.text = "";
        panel.SetActive(true);

        SetPlayerControl(false);
        input.ActivateInputField();
    }

    public void Close()
    {
        panel.SetActive(false);
        current = null;
        SetPlayerControl(true);
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
        }
    }

    public void Backspace()
    {
        if (input.text.Length > 0)
            input.text = input.text.Substring(0, input.text.Length - 1);
    }

    private void SetPlayerControl(bool enabled)
    {
        foreach (Behaviour b in disableWhileOpen) b.enabled = enabled;

        Cursor.lockState = enabled ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !enabled;
    }
}