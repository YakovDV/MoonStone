using System;
using UnityEngine;

public class RoverBase : MonoBehaviour
{
    [SerializeField] private ResourceUnloadZone _resourceUnloadZone;
    [SerializeField] private BaseMoneySpawner _moneySpawner;

    public event Action<int> ResourceDelivered;

    private void OnEnable()
    {
        _resourceUnloadZone.Unloaded += OnUnloaded;
    }

    private void OnDisable()
    {
        _resourceUnloadZone.Unloaded -= OnUnloaded;
    }

    public void Initialize()
    {
        
    }

    private void OnUnloaded(int value)
    {
        ResourceDelivered?.Invoke(value);
        _moneySpawner.Spawn(value);
    }
}