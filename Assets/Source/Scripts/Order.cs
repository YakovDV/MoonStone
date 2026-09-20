using System;

public class Order
{
    public float Progress { get; private set; }
    public float RequiredValue { get; }

    public bool IsCompleted => Progress >= RequiredValue;

    public event Action<float> ProgressChanged;
    public event Action Completed;

    public Order(float requiredValue)
    {
        RequiredValue = requiredValue;
    }

    public void AddProgress(float value)
    {
        if (value <= 0 || IsCompleted)
            return;

        Progress += value;
        ProgressChanged?.Invoke(Progress);

        if (IsCompleted)
            Completed?.Invoke();
    }
}
