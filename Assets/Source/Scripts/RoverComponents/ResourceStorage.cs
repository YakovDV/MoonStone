using System;
using UnityEngine;

public class ResourceStorage : MonoBehaviour, IStorage
{
    [SerializeField] private float _maxCapacity = 100f;

    private float _currentResourcesMass;
    private float _currentResourcesValue;

    public event Action<float> MassChanged;
    public event Action<float> ValueChanged;

    public bool CanAccept(ResourceConfig config)
    {
        return _currentResourcesMass + config.Mass <= _maxCapacity;
    }

    public bool TryAdd(ResourceConfig config)
    {
        if (CanAccept(config) == false)
            return false;

        AddResource(config);

        return true;
    }

    private void AddResource(ResourceConfig config)
    {
        _currentResourcesMass += config.Mass;
        _currentResourcesValue += config.Value;

        MassChanged?.Invoke(_currentResourcesMass);
        ValueChanged?.Invoke(_currentResourcesValue);
    }
}
