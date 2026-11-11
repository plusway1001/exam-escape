using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gunnode
{
    // a reusable "are you sure?" box for things you can't undo, like leaving a level.
    // the title and message are drawn into the background image, so only the buttons are wired here.
    // set No as first selected, so mashing Enter can't confirm by accident.
    public class ConfirmDialog : UIPanel
    {
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;

        // what to do on Yes / No. changed every time the dialog is used
        private Action onYes;
        private Action onNo;

        private void Awake()
        {
            // hook the buttons up once. they call whatever onYes/onNo currently is.
            yesButton.onClick.AddListener(() => onYes?.Invoke());
            noButton.onClick.AddListener(() => onNo?.Invoke());
        }

        public void Setup(Action onYes, Action onNo)
        {
            this.onYes = onYes;
            this.onNo = onNo;
        }
    }
}
