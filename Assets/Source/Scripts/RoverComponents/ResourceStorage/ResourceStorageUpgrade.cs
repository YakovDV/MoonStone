using UnityEngine;

public class ResourceStorageUpgrade : UpgradeableComponent
{
    [SerializeField] private ResourceStorage _storage;
    [SerializeField] private ResourceStorageUpgradeConfig _upgradeConfig;

    public override int MaxLevel => _upgradeConfig.MaxLevel;

    public override int GetUpgradeCost()
    {
        return CanUpgrade() ? _upgradeConfig.GetUpgradeCost(Level + 1) : 0;
    }

    public override int GetCurrentValue()
    {
        return (int)_storage.MaxCapacity;
    }

    protected override void ApplyLevel(int level)
    {
        float capacity = _storage.BaseCapacity + _upgradeConfig.GetTotalCapacityIncrease(level);

        _storage.SetMaxCapacity(capacity);
    }
}
