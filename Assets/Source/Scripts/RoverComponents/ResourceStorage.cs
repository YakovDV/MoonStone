using System;
using UnityEngine;

public class ResourceStorage : MonoBehaviour, IStorage
{
    [SerializeField] private float _maxCapacity = 100f;

    private float _currentResourcesMass;
    private int _currentResourcesValue;

    public float MaxCapacity => _maxCapacity;
    public float CurrentResourcesMass => _currentResourcesMass;
    public int CurrentResourcesValue => _currentResourcesValue;

    public event Action<float> MassChanged;
    public event Action<int> ValueChanged;

    public bool CanAccept(ResourceConfig config)
    {
        return _currentResourcesMass + config.Mass <= _maxCapacity;
    }

    public bool TryAdd(ResourceConfig config)
    {
        if (CanAccept(config) == false)
        {
            Debug.Log("Storage is full");
            return false;
        }

        AddResource(config);

        Debug.Log($"Added {config.Value} vale, {config.Mass} mass");

        return true;
    }

    public void Clear()
    {
        _currentResourcesMass = 0;
        _currentResourcesValue = 0;

        MassChanged?.Invoke(0);
        ValueChanged?.Invoke(0);
    }

    private void AddResource(ResourceConfig config)
    {
        _currentResourcesMass += config.Mass;
        _currentResourcesValue += config.Value;

        MassChanged?.Invoke(_currentResourcesMass);
        ValueChanged?.Invoke(_currentResourcesValue);
    }
}
