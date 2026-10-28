using System;
using UnityEngine;

public abstract class UpgradeableComponent : MonoBehaviour
{
    [SerializeField] private UpgradeType _upgradeType;

    public int Level { get; protected set; }
    public abstract int MaxLevel { get; }

    public UpgradeType UpgradeType => _upgradeType;

    private void OnValidate()
    {
        if (Level > MaxLevel)
        {
            Level = MaxLevel;

            Debug.LogError($"{UpgradeType} level can't be more than {MaxLevel}. Level set to MaxLevel", this);
        }
        else if (Level < 0)
        {
            Level = 1;

            Debug.Log($"{UpgradeType} level can't be less than 0. Level set to 1", this);
        }
    }

    public void Initialize(int level)
    {
        if (level < 1)
            throw new ArgumentOutOfRangeException($"{_upgradeType} level can't be less than zero");

        if (level > MaxLevel)
            throw new ArgumentOutOfRangeException($"{_upgradeType} level can't be more than {MaxLevel}");

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
