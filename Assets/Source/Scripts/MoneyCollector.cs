using System;
using UnityEngine;

public class MoneyCollector : MonoBehaviour
{
    public event Action<int> MoneyCollected;

    public void Collect(int value)
    {
        MoneyCollected?.Invoke(value);
    }
}