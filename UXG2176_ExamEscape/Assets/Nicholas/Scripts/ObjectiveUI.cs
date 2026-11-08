using TMPro;
using UnityEngine;

namespace Nicholas
{
    public class ObjectiveUI : MonoBehaviour
    {
        [SerializeField] private ObjectiveTracker tracker;
        [SerializeField] private Transform container;
        [SerializeField] private TMP_Text rowPrefab;
        [SerializeField] private Color doneColor = new Color(0.5f, 0.9f, 0.5f);
        [SerializeField] private Color todoColor = Color.white;

        private TMP_Text[] rows;

        private void Start()
        {
            rows = new TMP_Text[tracker.All.Count];
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = Instantiate(rowPrefab, container);
            }

            tracker.Changed += Refresh;
            Refresh();
        }

        private void OnDestroy()
        {
            if (tracker != null)
            {
                tracker.Changed -= Refresh;
            }
        }

        private void Refresh()
        {
            for (int i = 0; i < rows.Length; i++)
            {
                Objective o = tracker.All[i];
                bool done = tracker.IsComplete(o);
                rows[i].text = done ? $"[x] <s>{o.title}</s>" : $"[ ] {o.title}";
                rows[i].color = done ? doneColor : todoColor;
            }
        }
    }
}