using System;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    public int Value { get; private set; }

    public event Action<int> ValueChanged;

    public void Add(int value)
    {
        if (Value + value <= 0)
        {
            Value = 0;
            return;
        }

        Value += value;

        ValueChanged?.Invoke(Value);
    }
}
