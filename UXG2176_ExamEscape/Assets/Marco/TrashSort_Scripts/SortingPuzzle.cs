using System;
using System.Collections.Generic;
using UnityEngine;

public enum SubmitResult
{
    Correct,
    NotSortable,   // junk: no bin exists for this material
    WrongBin,      // recyclable, but put in the wrong bin
    WrongOrder     // right bin for the item, but not the item the sequence needs next
}

/// <summary>
/// Pure puzzle logic: generates a random sequence and judges submissions.
/// Knows nothing about timers, UI or doors, it only raises events.
/// </summary>
public class SortingPuzzle : MonoBehaviour
{
    [Tooltip("Materials that have a bin. Add Wood here (and a Wood bin) to make wood sortable.")]
    [SerializeField] private List<MaterialType> sortableMaterials = new()
    {
        MaterialType.Paper, MaterialType.Cardboard, MaterialType.Plastic
    };
    [SerializeField, Min(1)] private int minSequenceLength = 4;
    [SerializeField, Min(1)] private int maxSequenceLength = 6;

    private readonly List<MaterialType> sequence = new();

    public IReadOnlyList<MaterialType> Sequence => sequence;
    public int Progress { get; private set; }
    public bool IsSolved => sequence.Count > 0 && Progress >= sequence.Count;

    public event Action OnSequenceGenerated;
    public event Action<SubmitResult> OnSubmitted;
    public event Action OnProgressChanged;
    public event Action OnSolved;

    private void OnValidate()
    {
        if (maxSequenceLength < minSequenceLength) maxSequenceLength = minSequenceLength;
    }

    public void Generate()
    {
        sequence.Clear();
        Progress = 0;

        if (sortableMaterials.Count == 0)
        {
            Debug.LogError("SortingPuzzle: no sortable materials configured.", this);
            return;
        }

        int length = UnityEngine.Random.Range(minSequenceLength, maxSequenceLength + 1);
        for (int i = 0; i < length; i++)
            sequence.Add(sortableMaterials[UnityEngine.Random.Range(0, sortableMaterials.Count)]);

        OnSequenceGenerated?.Invoke();
    }

    public SubmitResult Submit(MaterialType binType, TrashItemData item)
    {
        if (IsSolved) return SubmitResult.Correct; // already finished, accept silently

        SubmitResult result;
        if (!sortableMaterials.Contains(item.material)) result = SubmitResult.NotSortable;
        else if (item.material != binType)              result = SubmitResult.WrongBin;
        else if (binType != sequence[Progress])         result = SubmitResult.WrongOrder;
        else                                            result = SubmitResult.Correct;

        if (result == SubmitResult.Correct)
        {
            Progress++;
            OnProgressChanged?.Invoke();
        }

        OnSubmitted?.Invoke(result);

        if (result == SubmitResult.Correct && IsSolved) OnSolved?.Invoke();
        return result;
    }
}
