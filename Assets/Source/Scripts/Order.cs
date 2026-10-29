using System;

public class Order
{
    public int Progress { get; private set; }
    public int RequiredValue { get; }

    public bool IsCompleted => Progress >= RequiredValue;

    public event Action<float> ProgressChanged;
    public event Action Completed;

    public Order(int requiredValue)
    {
        RequiredValue = requiredValue;
    }

    public void AddProgress(int value)
    {
        if (value <= 0 || IsCompleted)
            return;

        Progress += value;
        ProgressChanged?.Invoke(Progress);

        if (IsCompleted)
            Completed?.Invoke();
    }

    public void Reset()
    {
        Progress = 0;
        ProgressChanged?.Invoke(Progress);
    }
}
