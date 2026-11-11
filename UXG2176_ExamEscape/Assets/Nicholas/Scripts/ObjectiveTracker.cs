using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nicholas
{
    public class ObjectiveTracker : MonoBehaviour
    {
        public static ObjectiveTracker Instance { get; private set; }

        [SerializeField] private List<Objective> objectives = new();

        private readonly HashSet<Objective> completed = new();

        public IReadOnlyList<Objective> All => objectives;
        public event Action Changed;

        private void Awake() => Instance = this;

        public bool IsComplete(Objective o) => completed.Contains(o);

        public void Complete(Objective o)
        {
            if (o == null || !completed.Add(o)) return;
            Changed?.Invoke();

            if (objectives.IndexOf(o) == objectives.Count - 1)
            {
                Gunnode.GameManager.CompleteLevel();
            }
        }
    }
}