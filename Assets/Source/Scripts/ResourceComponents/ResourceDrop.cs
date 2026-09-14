using System;
using UnityEngine;

[Serializable]
public class ResourceDrop
{
    [SerializeField] private ResourceConfig _resource;
    [SerializeField, Range(0f, 1f)] private float _chance = 1f;
    [SerializeField] private int _minAmount = 1;
    [SerializeField] private int _maxAmount = 10;

    public ResourceConfig Resource => _resource;
    public float Chance => _chance;
    public int MinAmount => _minAmount;
    public int MaxAmount => _maxAmount;
}