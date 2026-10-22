using System;

[Serializable]
public struct UpgradeData
{
    public UpgradeEntry[] Upgrades;

    public bool TryGetLevel(UpgradeType type, out int level)
    {
        foreach (UpgradeEntry entry in Upgrades)
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
    public UpgradeType Type;
    public int Level;
}