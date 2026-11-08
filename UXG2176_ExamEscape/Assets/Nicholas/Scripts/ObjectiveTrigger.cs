using UnityEngine;

namespace Nicholas
{
    public class ObjectiveTrigger : MonoBehaviour
    {
        [SerializeField] private Objective objective;

        public void Complete() => ObjectiveTracker.Instance.Complete(objective);
    }
}