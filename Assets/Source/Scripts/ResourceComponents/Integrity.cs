using System;
using UnityEngine;

public class Integrity : MonoBehaviour
{
    [SerializeField] private int _value;

    public int CurrentValue { get; private set; }

    public event Action<int> ValueChanged;
    public event Action Destroyed;

    private void Awake()
    {
        CurrentValue = _value;
    }

    public void TryMine(int damage)
    {
        if (damage <= 0)
            return;

        CurrentValue -= damage;

        if (CurrentValue < 0)
        {
            CurrentValue = 0;
            Destroyed?.Invoke();
        }

        Debug.Log(CurrentValue);
        ValueChanged?.Invoke(CurrentValue);
    }
}