using System;
using UnityEngine;

/// <summary>Countdown timer with penalties. Raises events, knows nothing about UI or game rules.</summary>
public class GameTimer : MonoBehaviour
{
    [SerializeField] private float startSeconds = 180f;

    public float Remaining { get; private set; }
    public bool IsRunning { get; private set; }

    public event Action<float> OnTimeChanged;
    public event Action<float> OnPenalty;
    public event Action OnExpired;

    public void Begin()
    {
        Remaining = startSeconds;
        IsRunning = true;
        OnTimeChanged?.Invoke(Remaining);
    }

    public void Stop() => IsRunning = false;

    public void Subtract(float seconds)
    {
        if (!IsRunning) return;

        Remaining = Mathf.Max(0f, Remaining - seconds);
        OnPenalty?.Invoke(seconds);
        OnTimeChanged?.Invoke(Remaining);
        CheckExpired();
    }

    private void Update()
    {
        if (!IsRunning) return;

        Remaining = Mathf.Max(0f, Remaining - Time.deltaTime);
        OnTimeChanged?.Invoke(Remaining);
        CheckExpired();
    }

    private void CheckExpired()
    {
        if (Remaining > 0f) return;

        IsRunning = false;
        OnExpired?.Invoke();
    }
}
