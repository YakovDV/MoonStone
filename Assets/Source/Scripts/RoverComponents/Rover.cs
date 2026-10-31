using UnityEngine;
using Zenject;

public class Rover : MonoBehaviour
{
    [SerializeField] private UpgradeableComponent[] _upgradeableComponents;
    [SerializeField] private PlayerMover _playerMover;
    [SerializeField] private ResourceStorage _resourceStorage;

    public void ResetState()
    {
        _playerMover.ResetPosition();
        _resourceStorage.Clear();
    }

    public void InitializeUpgrades(UpgradeData upgradeData)
    {
        foreach (var upgradeableComponent in _upgradeableComponents)
        {
            if (upgradeData.TryGetLevel(upgradeableComponent.UpgradeType, out int level))
                upgradeableComponent.Initialize(level);
        }
    }

    public UpgradeData GetUpgradeData()
    {
        UpgradeEntry[] entries = new UpgradeEntry[_upgradeableComponents.Length];

        for (int i = 0; i < _upgradeableComponents.Length; i++)
        {
            UpgradeableComponent component = _upgradeableComponents[i];

            entries[i] = new UpgradeEntry(component.UpgradeType, component.Level);
        }

        return new UpgradeData(entries);
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