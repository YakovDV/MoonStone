using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Wallet _wallet;
    [SerializeField] private WalletView[] _walletViews;

    [SerializeField] private RoverBase _roverBase;
    [SerializeField] private Rover _rover;
    [SerializeField] private UpgradeData _upgradeData;

    [SerializeField] private MoneyCollector _moneyCollector;
    [SerializeField] private OrderConfig _config;
    [SerializeField] private ProgressBar _progressBar;
    [SerializeField] private ProgressView _progressView;

    [SerializeField] private PanelSwitcher _panelSwitcher;

    [SerializeField] private CanvasGroup _winPanel;
    [SerializeField] private CanvasGroup _playerHUD;

    private Order _order;

    private void Awake()
    {
        _order = new(_config.TargetValue);
        _winPanel.alpha = 0;
        _playerHUD.alpha = 1;

        if (_wallet != null)
            foreach (WalletView walletView in _walletViews)
                walletView.SetWallet(_wallet);

        if (_progressBar != null)
            _progressBar.SetOrder(_order);

        if (_progressView != null)
            _progressView.SetOrder(_order);

        _rover.Initialize(_upgradeData);
    }

    private void Start()
    {
        _panelSwitcher.Show(ScreenID.HUD);
    }

    private void OnEnable()
    {
        _roverBase.ResourceDelivered += OnResourceDelivered;
        _moneyCollector.MoneyCollected += OnMoneyCollected;
        _order.Completed += OnOrderComleted;
    }

    private void OnDisable()
    {
        _roverBase.ResourceDelivered -= OnResourceDelivered;
        _moneyCollector.MoneyCollected -= OnMoneyCollected;
        _order.Completed -= OnOrderComleted;
    }

    private void OnResourceDelivered(int value)
    {
        _order.AddProgress(value);
    }

    private void OnMoneyCollected(int value)
    {
        _wallet.Add(value);
    }

    private void OnOrderComleted()
    {
        _winPanel.alpha = 1;
        _playerHUD.alpha = 0;
    }
}
