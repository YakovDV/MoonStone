using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceStorageUpgradeConfig", menuName = "Incremental Mining/Upgrades/Container")]
public class ResourceStorageUpgradeConfig : ScriptableObject
{
    [SerializeField, Min(1)] private int _maxLevel = 100;
    [SerializeField] private CapacityGroup[] _capacityGroups;
    [SerializeField] private CostGroup[] _costGroups;

    public int MaxLevel => _maxLevel;

    public int GetUpgradeCost(int level)
    {
        if (level < 2 || level > MaxLevel)
            throw new ArgumentOutOfRangeException(nameof(level));

        foreach (CostGroup group in _costGroups)
        {
            if (level >= group.StartLevel && level <= group.EndLevel)
            {
                return group.Cost;
            }
        }

        throw new InvalidOperationException($"No cost group configured for level {level}.");
    }

    public float GetTotalCapacityIncrease(int level)
    {
        if (level < 1 || level > MaxLevel)
            throw new ArgumentOutOfRangeException(nameof(level));

        float totalIncrease = 0f;

        for (int currentLevel = 2; currentLevel <= level; currentLevel++)
        {
            totalIncrease += GetCapacityIncrease(currentLevel);
        }

        return totalIncrease;
    }

    private float GetCapacityIncrease(int level)
    {
        foreach (CapacityGroup group in _capacityGroups)
        {
            if (level >= group.StartLevel &&
                level <= group.EndLevel)
            {
                return group.CapacityIncrease;
            }
        }

        throw new InvalidOperationException($"No capacity group configured for level {level}.");
    }

    [Serializable]
    public struct CapacityGroup
    {
        [Min(2)] public int StartLevel;
        [Min(2)] public int EndLevel;
        [Min(0.1f)] public float CapacityIncrease;
    }

    [Serializable]
    public struct CostGroup
    {
        [Min(2)] public int StartLevel;
        [Min(2)] public int EndLevel;
        [Min(0)] public int Cost;
    }
}