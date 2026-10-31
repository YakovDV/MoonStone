using System;
using UnityEngine;

[Serializable]
public struct UpgradeData
{
    public UpgradeEntry[] _upgrades;

    public UpgradeData(UpgradeEntry[] upgrades)
    {
        if (upgrades == null)
        {
            _upgrades = Array.Empty<UpgradeEntry>();
            return;
        }

        _upgrades = new UpgradeEntry[upgrades.Length];
        Array.Copy(upgrades, _upgrades, upgrades.Length);
    }

    public bool TryGetLevel(UpgradeType type, out int level)
    {
        foreach (UpgradeEntry entry in _upgrades)
        {
            if (entry.Type != type)
                continue;

            level = entry.Level;
            return true;
        }

        level = 0;
        return false;
    }
}

[Serializable]
public struct UpgradeEntry
{
    [SerializeField] private UpgradeType _type;
    [SerializeField] private int _level;

    public UpgradeType Type => _type;
    public int Level => _level;

    public UpgradeEntry(UpgradeType type, int level)
    {
        _type = type;
        _level = level;
    }
}