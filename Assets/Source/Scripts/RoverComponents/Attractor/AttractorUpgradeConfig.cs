using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AttractorUpgradeConfig", menuName = "Incremental Mining/Upgrades/Attractor")]
public class AttractorUpgradeConfig : ScriptableObject
{
    [SerializeField] private LevelData[] _levels;

    public int MaxLevel => _levels.Length;

    public LevelData GetLevelData(int level)
    {
        if (level < 1 || level > MaxLevel)
            throw new ArgumentOutOfRangeException(nameof(level));

        return _levels[level - 1];
    }

    [Serializable]
    public struct LevelData
    {
        [Min(1)] public int MaxResourceTier;
        [Min(0)] public int UpgradeCost;
    }
}