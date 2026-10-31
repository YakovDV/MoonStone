using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UpgradeShop : MonoBehaviour
{
    [Inject] private Wallet _wallet;
    [Inject] private PanelSwitcher _panelSwitcher;
    [Inject] private PlayerInputReader _playerInputReader;

    [SerializeField] private CameraMover _cameraMover;
    [SerializeField] private Button _closeButton;

    [SerializeField] private UpgradeCard[] _upgradeCards;

    [SerializeField] private ParticleEffectPlayer _particleEffectPlayer;

    private Rover _rover;

    public event Action CantPurchase;
    public event Action CantUpgrade;

    private void OnEnable()
    {
        _closeButton.onClick.AddListener(OnCloseClick);

        foreach (var upgradeCard in _upgradeCards)
            upgradeCard.PurchaseRequested += OnPurchaseRequested;
    }

    private void OnDisable()
    {
        _closeButton.onClick.RemoveListener(OnCloseClick);

        foreach (var upgradeCard in _upgradeCards)
            upgradeCard.PurchaseRequested -= OnPurchaseRequested;
    }

    public void Open(Rover rover)
    {
        if (rover == null)
        {
            Debug.LogError("Cannot open UpgradeShop without Rover.");
            return;
        }

        _cameraMover.ChangePositionSmooth(CameraPositionType.AlternativePosition);

        _rover = rover;
        RefreshCards();
        _panelSwitcher.Show(ScreenID.Shop);

        _playerInputReader.DisableGameplayInput();
    }

    public void Close()
    {
        _cameraMover.ChangePositionSmooth(CameraPositionType.NormalPosition);

        _rover = null;
        _panelSwitcher.Show(ScreenID.HUD);

        _playerInputReader.EnableGameplayInput();
    }

    private void OnCloseClick()
    {
        Close();
    }

    private void OnPurchaseRequested(UpgradeType upgradeType)
    {
        if (_rover == null)
            return;

        UpgradeableComponent upgradeableComponent = _rover.GetUpgrade(upgradeType);

        if (upgradeableComponent == null)
            return;

        if (upgradeableComponent.CanUpgrade() == false)
        {
            CantUpgrade?.Invoke();
            return;
        }

        int upgradeCost = upgradeableComponent.GetUpgradeCost();

        if (_wallet.CanSpend(upgradeCost) == false)
        {
            CantPurchase?.Invoke();
            return;
        }

        if (_wallet.TrySpend(upgradeCost) == false)
        {
            CantPurchase?.Invoke();
            return;
        }

        if (upgradeableComponent.TryUpgrade() == false)
        {

            _wallet.Add(upgradeCost);
            CantUpgrade?.Invoke();
        }

        _particleEffectPlayer.PlayEffectAtPoint(_rover.transform.position);

        RefreshCards();
    }

    private void RefreshCards()
    {
        foreach (var upgradeCard in _upgradeCards)
        {
            UpgradeableComponent component = _rover.GetUpgrade(upgradeCard.UpgradeType);

            if (component == null)
            {
                Debug.LogError($"Upgrade not found: {upgradeCard.UpgradeType}");
                continue;
            }

            upgradeCard.UpdateView(component, _wallet.Value >= component.GetUpgradeCost());
        }
    }
}
