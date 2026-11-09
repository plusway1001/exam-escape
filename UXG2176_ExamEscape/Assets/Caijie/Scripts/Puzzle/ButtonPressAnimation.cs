using System.Collections;
using UnityEngine;

namespace Caijie
{
    // after consideration, decide to use script instead of animator 
    // this is only to control button mesh: push in, stay slicked, or pop back out.
    public class ButtonPressAnimation : MonoBehaviour
    {
        // how far the mesh sinks in when pressed, in its parent's space.
        [SerializeField] private Vector3 pressOffset = new Vector3(0f, -0.01f, 0f);

        // how long one movement (down or up) takes, in seconds
        [SerializeField] private float pressTime = 0.08f;

        private Vector3 restPosition;   // sticking out (default position)
        private Vector3 downPosition;   // pushed in
        private Coroutine routine;      // the animation that's playing right now, if any

        private void Awake()
        {
            // remember where the mesh starts, and work out where "pushed in" is
            restPosition = transform.localPosition;
            downPosition = restPosition + pressOffset;
        }

        public void Click()
        {
            Play(PressAndRelease());
        }

        // true = pushed and stayed (correct button)
        // false = pop back out after a mistake pressed
        public void SetHeldDown(bool held)
        {
            Vector3 target = restPosition;
            if (held)
                target = downPosition;

            Play(Move(transform.localPosition, target));
        }

        // stops any animation already playing before starting the new one
        // this stops two coroutines from fighting over the mesh
        private void Play(IEnumerator animation)
        {
            if (routine != null)
                StopCoroutine(routine);

            routine = StartCoroutine(animation);
        }

        private IEnumerator PressAndRelease()
        {
            // yield return another coroutine, so it wait until that one finishes
            yield return Move(transform.localPosition, downPosition);
            yield return Move(downPosition, restPosition);
        }

        // slides the mesh from one spot to another over pressTime seconds
        private IEnumerator Move(Vector3 from, Vector3 to)
        {
            for (float t = 0f; t < 1f; t += Time.deltaTime / pressTime)
            {
                transform.localPosition = Vector3.Lerp(from, to, t);
                yield return null;    // wait one frame
            }

            // snap button on the end position
            transform.localPosition = to;
        }
    }
}