using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CrusherUpgradeConfig", menuName = "Incremental Mining/Upgrades/Crusher")]
public class CrusherUpgradeConfig : ScriptableObject
{
    [SerializeField, Min(1)] private int _maxLevel = 100;
    [SerializeField] private DamageGroup[] _damageGroups;
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

    public int GetTotalDamageIncrease(int level)
    {
        if (level < 1 || level > MaxLevel)
            throw new ArgumentOutOfRangeException(nameof(level));

        int totalIncrease = 0;

        for (int currentLevel = 2; currentLevel <= level; currentLevel++)
        {
            totalIncrease += GetDamageIncrease(currentLevel);
        }

        return totalIncrease;
    }

    private int GetDamageIncrease(int level)
    {
        foreach (DamageGroup group in _damageGroups)
        {
            if (level >= group.StartLevel &&
                level <= group.EndLevel)
            {
                return group.DamageIncrease;
            }
        }

        throw new InvalidOperationException(
            $"No damage group configured for level {level}.");
    }

    [Serializable]
    public struct DamageGroup
    {
        [Min(2)] public int StartLevel;
        [Min(2)] public int EndLevel;
        [Min(1)] public int DamageIncrease;
    }

    [Serializable]
    public struct CostGroup
    {
        [Min(2)] public int StartLevel;
        [Min(2)] public int EndLevel;
        [Min(0)] public int Cost;
    }
}