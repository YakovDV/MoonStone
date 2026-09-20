using System;

public class Mission
{
    public Order CurrentOrder { get; }

    public event Action Completed;

    public Mission(OrderConfig config)
    {
        CurrentOrder = new Order(config.TargetValue);
        CurrentOrder.Completed += OnOrderCompleted;
    }

    public void AddProgress(float value)
    {
        CurrentOrder.AddProgress(value);
    }

    private void OnOrderCompleted()
    {
        Completed?.Invoke();
        CurrentOrder.Completed -= OnOrderCompleted;
    }
}