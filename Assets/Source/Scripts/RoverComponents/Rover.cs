using UnityEngine;

public class Rover : MonoBehaviour
{
    [SerializeField] private UpgradeableComponent[] _upgradeableComponents;
    [SerializeField] private PlayerMover _playerMover;
    [SerializeField] private ResourceStorage _resourceStorage;

    public void Initialize(UpgradeData upgradeData)
    {
        foreach (var upgradeableComponent in _upgradeableComponents)
        {
            if (upgradeData.TryGetLevel(upgradeableComponent.UpgradeType, out int level))
                upgradeableComponent.Initialize(level);
        }
    }

    public void ResetState()
    {
        _playerMover.ResetPosition();
        _resourceStorage.Clear();
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