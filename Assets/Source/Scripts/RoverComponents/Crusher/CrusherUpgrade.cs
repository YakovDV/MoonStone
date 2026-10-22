using UnityEngine;

public class CrusherUpgrade : UpgradeableComponent
{
    [SerializeField] private Crusher _crusher;
    [SerializeField] private CrusherUpgradeConfig _upgradeConfig;

    public override int MaxLevel => _upgradeConfig.MaxLevel;

    public override int GetUpgradeCost()
    {
        return CanUpgrade() ? _upgradeConfig.GetUpgradeCost(Level + 1) : 0;
    }

    public override int GetCurrentValue()
    {
        return _crusher.Damage;
    }

    protected override void ApplyLevel(int level)
    {
        int damage = _crusher.BaseDamage + _upgradeConfig.GetTotalDamageIncrease(level);

        _crusher.SetDamage(damage);
    }
}
