using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private PanelSwitcher _panelSwitcher;

    [SerializeField] private MainMenuPanel _mainMenuPanel;
    [SerializeField] private SettingsPanel _settingPanel;

    private void Awake()
    {
        _panelSwitcher.Show(ScreenID.MainMenu);
    }

    private void OnEnable()
    {
        _mainMenuPanel.StartRequested += OnStartRequested;
        _mainMenuPanel.LeaderboardRequested += OnLeaderboardRequested;
        _mainMenuPanel.SettingsRequested += OnSettingsRequested;

        _settingPanel.ReturnRequested += OnReturnRequested;
    }

    private void OnDisable()
    {
        _mainMenuPanel.StartRequested -= OnStartRequested;
        _mainMenuPanel.LeaderboardRequested -= OnLeaderboardRequested;
        _mainMenuPanel.SettingsRequested -= OnSettingsRequested;

        _settingPanel.ReturnRequested -= OnReturnRequested;
    }

    private void OnStartRequested()
    {

    }

    private void OnLeaderboardRequested()
    {

    }

    private void OnSettingsRequested()
    {
        _panelSwitcher.Show(ScreenID.Settings);
    }

    private void OnReturnRequested()
    {
        _panelSwitcher.Show(_panelSwitcher.PreviousScreen);
    }
}

public class MainMenuPanel : UIPanel
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _leaderboardButton;
    [SerializeField] private Button _settingsButton;

    public event Action StartRequested;
    public event Action LeaderboardRequested;
    public event Action SettingsRequested;

    private void OnEnable()
    {
        _startButton.onClick?.AddListener(RequestStart);
        _leaderboardButton.onClick?.AddListener(RequestLeaderboard);
        _settingsButton.onClick?.AddListener(RequestSettings);
    }

    private void RequestStart()
    {
        StartRequested?.Invoke();
    }

    private void RequestLeaderboard()
    {
        LeaderboardRequested?.Invoke();
    }

    private void RequestSettings()
    {
        SettingsRequested?.Invoke();
    }
}

public class SettingsPanel : UIPanel
{
    [SerializeField] private Button _returnButton;

    public event Action ReturnRequested;

    private void OnEnable()
    {
        _returnButton.onClick?.AddListener(RequestReturn);
    }

    private void OnDisable()
    {
        _returnButton.onClick?.RemoveListener(RequestReturn);
    }

    private void RequestReturn()
    {
        ReturnRequested?.Invoke();
    }
}