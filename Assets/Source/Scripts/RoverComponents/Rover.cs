using UnityEngine;

public class Rover : MonoBehaviour
{
    [SerializeField] private UpgradeableComponent[] _upgradeableComponents;

    public void Initialize(UpgradeData upgradeData)
    {
        foreach (var upgradeableComponent in _upgradeableComponents)
        {
            if (upgradeData.TryGetLevel(upgradeableComponent.UpgradeType, out int level))
                upgradeableComponent.Initialize(level);
        }
    }

    public UpgradeableComponent GetUpgrade(UpgradeType type)
    {
        foreach (var component in _upgradeableComponents)
        {
            if (component.UpgradeType == type)
                return component;
        }

        return null;
    }
}

public enum UpgradeType
{
    Crusher,
    Container,
    Attractor
}