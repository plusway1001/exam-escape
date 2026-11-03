using UnityEngine;

/// <summary>
/// Put this on a child object with an "Is Trigger" collider covering the bin opening.
/// Correct item = consumed. Anything else = pushed back out (never lost, so the puzzle stays solvable).
/// </summary>
[RequireComponent(typeof(Collider))]
public class TrashBin : MonoBehaviour
{
    [SerializeField] private MaterialType binType;
    [SerializeField] private SortingPuzzle puzzle;

    [Header("Rejection")]
    [Tooltip("Where rejected items reappear. Its forward direction is the push direction. Falls back to the bin itself.")]
    [SerializeField] private Transform ejectPoint;
    [SerializeField] private float ejectSpeed = 4f;

    [Header("Audio (optional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctClip;
    [SerializeField] private AudioClip wrongClip;

    public MaterialType BinType => binType;

    private void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponentInParent<TrashItem>();
        if (item == null || !item.CanBeSubmitted) return;

        var result = puzzle.Submit(binType, item.Data);

        if (result == SubmitResult.Correct)
        {
            Play(correctClip);
            item.Consume();
        }
        else
        {
            Play(wrongClip);
            Transform origin = ejectPoint != null ? ejectPoint : transform;
            Vector3 velocity = (origin.forward + Vector3.up) * ejectSpeed;
            item.Eject(origin.position, velocity);
        }
    }

    private void Play(AudioClip clip)
    {
        if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
    }
}
