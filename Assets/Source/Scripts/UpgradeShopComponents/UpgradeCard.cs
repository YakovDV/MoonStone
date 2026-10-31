using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] private UpgradeType _upgradeType;
    [SerializeField] private Button _requestUpgrade;
    [SerializeField] private Slider _levelSlider;
    [SerializeField] private TextMeshProUGUI _upgradeCostText;

    [SerializeField] private TextMeshProUGUI _currentLevel;
    [SerializeField] private TextMeshProUGUI _cuerrentValue;

    [SerializeField] private string _levelText = "lvl. ";

    [SerializeField] private Image _maxUpgradeLevelImage;
    [SerializeField] private Image _moneyIcon;
    [SerializeField] private Image _notEnoughMoney;

    public event Action<UpgradeType> PurchaseRequested;

    public UpgradeType UpgradeType => _upgradeType;

    private void OnEnable()
    {
        _requestUpgrade.onClick.AddListener(OnUpgradeRequested);
    }

    private void OnDisable()
    {
        _requestUpgrade.onClick.RemoveListener(OnUpgradeRequested);
    }

    public void UpdateView(UpgradeableComponent upgradeableComponent, bool isEnoughMoney)
    {
        _levelSlider.maxValue = upgradeableComponent.MaxLevel;
        _levelSlider.value = upgradeableComponent.Level;

        if (upgradeableComponent.Level >= upgradeableComponent.MaxLevel)
        {
            _maxUpgradeLevelImage.enabled = true;
            _moneyIcon.enabled = false;
        }
        else
        {
            _maxUpgradeLevelImage.enabled = false;
            _moneyIcon.enabled = true;

            if (isEnoughMoney == true)
            {
                _moneyIcon.enabled = true;
                _notEnoughMoney.enabled = false;
            }
            else
            {
                _moneyIcon.enabled = false;
                _notEnoughMoney.enabled = true;
            }
        }

        _currentLevel.text = _levelText + upgradeableComponent.Level.ToString();
        _cuerrentValue.text = upgradeableComponent.GetCurrentValue().ToString();

        _upgradeCostText.text = upgradeableComponent.GetUpgradeCost().ToString();
    }

    private void OnUpgradeRequested()
    {
        PurchaseRequested?.Invoke(_upgradeType);
    }
}
