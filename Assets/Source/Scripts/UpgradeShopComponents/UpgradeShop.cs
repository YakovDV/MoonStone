using System;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeShop : MonoBehaviour
{
    [SerializeField] private Wallet _wallet;
    [SerializeField] private CameraMover _cameraMover;
    [SerializeField] private PanelSwitcher _panelSwitcher;
    [SerializeField] private Button _closeButton;

    [SerializeField] private UpgradeCard[] _upgradeCards;

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
    }

    public void Close()
    {
        _cameraMover.ChangePositionSmooth(CameraPositionType.NormalPosition);

        _rover = null;
        _panelSwitcher.Show(ScreenID.HUD);
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
            Debug.Log("Cant upgrade");

            CantUpgrade?.Invoke();
            return;
        }

        int upgradeCost = upgradeableComponent.GetUpgradeCost();

        if (_wallet.CanSpend(upgradeCost) == false)
        {
            Debug.Log("Not enough money");

            CantPurchase?.Invoke();
            return;
        }

        if (_wallet.TrySpend(upgradeCost) == false)
        {
            Debug.Log("Cant spend");

            CantPurchase?.Invoke();
            return;
        }

        if (upgradeableComponent.TryUpgrade() == false)
        {
            Debug.Log("Must be upgraded");

            _wallet.Add(upgradeCost);
            CantUpgrade?.Invoke();
        }

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

            upgradeCard.UpdateView(component);
        }
    }
}
