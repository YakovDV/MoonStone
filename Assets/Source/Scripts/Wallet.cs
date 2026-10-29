using System;
using UnityEngine;

public class Wallet
{
    public int Value { get; private set; }

    public event Action<int> ValueChanged;

    public void RestoreValue(int value)
    {
        Value = Mathf.Max(0, value);
        ValueChanged?.Invoke(Value);
    }

    public void Add(int value)
    {
        if (value <= 0)
        {
            return;
        }

        Value += value;

        ValueChanged?.Invoke(Value);
    }

    public bool CanSpend(int value)
    {
        if (value <= 0)
            return false;

        if (Value - value < 0)
            return false;

        return true;
    }

    public bool TrySpend(int value)
    {
        if (value <= 0)
            return false;

        if (Value - value < 0)
            return false;

        Value -= value;
        ValueChanged?.Invoke(Value);

        return true;
    }
}
