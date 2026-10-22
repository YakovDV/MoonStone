using UnityEngine;

public class AttractorUpgrade : UpgradeableComponent
{
    [SerializeField] private RoverAttractor _attractor;
    [SerializeField] private AttractorUpgradeConfig _upgradeConfig;

    public override int MaxLevel => _upgradeConfig.MaxLevel;

    public override int GetUpgradeCost()
    {
        if (Level >= MaxLevel)
            return 0;

        return _upgradeConfig.GetLevelData(Level + 1).UpgradeCost;
    }

    public override int GetCurrentValue()
    {
        return _attractor.MaxResourceTier;
    }

    protected override void ApplyLevel(int level)
    {
        int tier = _upgradeConfig.GetLevelData(level).MaxResourceTier;
        _attractor.SetMaxResourceTier(tier);
    }
}
