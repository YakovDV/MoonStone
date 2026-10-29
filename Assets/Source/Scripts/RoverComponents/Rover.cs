using UnityEngine;
using Zenject;

public class Rover : MonoBehaviour
{
    [SerializeField] private UpgradeableComponent[] _upgradeableComponents;
    [SerializeField] private PlayerMover _playerMover;
    [SerializeField] private ResourceStorage _resourceStorage;

    private UpgradeData _upgradeData;

    private void Start()
    {
        Initialize();
    }

    public void ResetState()
    {
        _playerMover.ResetPosition();
        _resourceStorage.Clear();
    }

    private void Initialize()
    {
        foreach (var upgradeableComponent in _upgradeableComponents)
        {
            if (_upgradeData.TryGetLevel(upgradeableComponent.UpgradeType, out int level))
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

    [Inject]
    private void Construct(UpgradeData upgradeData)
    {
        _upgradeData = upgradeData;
    }
}

public enum UpgradeType
{
    Crusher,
    Container,
    Attractor
}