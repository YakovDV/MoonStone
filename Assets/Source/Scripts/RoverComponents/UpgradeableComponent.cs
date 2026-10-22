using System;
using UnityEngine;

public abstract class UpgradeableComponent : MonoBehaviour
{
    [SerializeField] private UpgradeType _upgradeType;

    public int Level { get; protected set; }
    public abstract int MaxLevel { get; }

    public UpgradeType UpgradeType => _upgradeType;

    public void Initialize(int level)
    {
        if (level < 1 || level > MaxLevel)
            throw new ArgumentOutOfRangeException(nameof(level));

        Level = level;

        ApplyLevel(level);
    }

    public bool CanUpgrade()
    {
        if (Level >= MaxLevel)
        {
            return false;
        }

        return true;
    }

    public bool TryUpgrade()
    {
        if (Level >= MaxLevel)
        {
            return false;
        }

        Level++;

        ApplyLevel(Level);

        return true;
    }

    public abstract int GetUpgradeCost();
    public abstract int GetCurrentValue();
    protected abstract void ApplyLevel(int level);
}
