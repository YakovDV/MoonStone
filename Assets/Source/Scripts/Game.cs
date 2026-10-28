using UnityEngine;
using Zenject;

public class Game : MonoBehaviour
{
    [Inject] private Wallet _wallet;

    [Inject] private Rover _rover;
    [Inject] private RoverBase _roverBase;

    [Inject] private UpgradeData _upgradeData;
    [Inject] private OrderConfig _config;

    [Inject] private PanelSwitcher _panelSwitcher;

    [Inject] private BaseMoneySpawner _baseMoneySpawner;
    [Inject] private MoneyCollector _moneyCollector;

    [Inject] private PlayerInputReader _playerInputReader;

    [Inject] private LevelPopulation _levelPopulation;

    [Inject] private TerrainTrackMask _terrainTrackMask;

    [SerializeField] private ProgressBar _progressBar;
    [SerializeField] private ProgressView _progressView;

    [SerializeField] private StartGamePanel _startGamePanel;
    [SerializeField] private HUDPanel _hudPanel;
    [SerializeField] private PausePanel _pausePanel;
    [SerializeField] private CompletePanel _completePanel;

    private Order _order;
    private int _remainingMoneyBonus;

    private void Awake()
    {
        _order = new(_config.TargetValue);
        _startGamePanel.Initilize(_config);

        if (_progressBar != null)
            _progressBar.SetOrder(_order);

        if (_progressView != null)
            _progressView.SetOrder(_order);

        _rover.Initialize(_upgradeData);
        _levelPopulation.Initialize();
    }

    private void Start()
    {
        _panelSwitcher.Show(ScreenID.StartScreen);
    }

    private void OnEnable()
    {
        _startGamePanel.StartReqested += OnStartReqested;
        _hudPanel.PauseRequested += OnPauseRequested;

        _pausePanel.ResumeRequested += OnResumeRequested;
        _pausePanel.ExitRequested += OnExitRequested;

        _completePanel.RestartRequested += OnRestartRequested;
        _completePanel.ExitRequested += OnExitRequested;

        _roverBase.ResourceDelivered += OnResourceDelivered;
        _moneyCollector.MoneyCollected += OnMoneyCollected;
        _order.Completed += OnOrderComleted;
    }

    private void OnDisable()
    {
        _startGamePanel.StartReqested -= OnStartReqested;
        _hudPanel.PauseRequested -= OnPauseRequested;

        _pausePanel.ResumeRequested -= OnResumeRequested;
        _pausePanel.ExitRequested -= OnExitRequested;

        _completePanel.RestartRequested -= OnRestartRequested;
        _completePanel.ExitRequested -= OnExitRequested;

        _roverBase.ResourceDelivered -= OnResourceDelivered;
        _moneyCollector.MoneyCollected -= OnMoneyCollected;
        _order.Completed -= OnOrderComleted;
    }

    private void OnStartReqested()
    {
        StartOrder();
    }

    private void OnPauseRequested()
    {
        _panelSwitcher.Show(ScreenID.Pause);
        Time.timeScale = 0f;
    }

    private void OnResumeRequested()
    {
        _panelSwitcher.Show(ScreenID.HUD);
        Time.timeScale = 1f;
    }

    private void OnExitRequested()
    {
        _levelPopulation.DespawnAll();
        _baseMoneySpawner.DespawnAll();

        Time.timeScale = 1f;
        _playerInputReader.DisableGameplayInput();
    }

    private void OnRestartRequested()
    {
        _levelPopulation.DespawnAll();
        _baseMoneySpawner.DespawnAll();

        _order.Reset();
        _rover.ResetState();

        StartOrder();
    }

    private void StartOrder()
    {
        _levelPopulation.SpawnInitial();
        _remainingMoneyBonus = 0;

        _terrainTrackMask.ClearMask();

        _panelSwitcher.Show(ScreenID.HUD);
        _playerInputReader.EnableGameplayInput();
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
        _playerInputReader.DisableGameplayInput();

        _remainingMoneyBonus = _baseMoneySpawner.GetRemainingWorth();
        _completePanel.ShowBonus(_remainingMoneyBonus);
        _wallet.Add(_remainingMoneyBonus);

        _baseMoneySpawner.DespawnAll();

        _panelSwitcher.Show(ScreenID.Complete);
    }
}
