using System;
using UnityEngine;
using UnityEngine.UI;

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
