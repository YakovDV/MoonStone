using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private PanelSwitcher _panelSwitcher;

    [SerializeField] private MainMenuPanel _mainMenuPanel;
    [SerializeField] private SettingsPanel _settingPanel;
    [SerializeField] private LeaderboardPanel _leaderboardPanel;

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
        _leaderboardPanel.ReturnRequested += OnReturnRequested;
    }

    private void OnDisable()
    {
        _mainMenuPanel.StartRequested -= OnStartRequested;
        _mainMenuPanel.LeaderboardRequested -= OnLeaderboardRequested;
        _mainMenuPanel.SettingsRequested -= OnSettingsRequested;

        _settingPanel.ReturnRequested -= OnReturnRequested;
        _leaderboardPanel.ReturnRequested -= OnReturnRequested;
    }

    private void OnStartRequested()
    {

    }

    private void OnLeaderboardRequested()
    {
        _panelSwitcher.Show(ScreenID.Leaderboard);
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
