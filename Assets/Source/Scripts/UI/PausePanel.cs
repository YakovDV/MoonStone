using System;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : UIPanel
{
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _settingsButton;

    public event Action ResumeRequested;
    public event Action ExitRequested;
    public event Action SettingsRequested;

    private void OnEnable()
    {
        _resumeButton.onClick?.AddListener(RequestResume);
        _exitButton.onClick?.AddListener(RequestExit);
        _settingsButton.onClick?.AddListener(RequestSettings);
    }

    private void RequestResume()
    {
        ResumeRequested?.Invoke();
    }

    private void RequestExit()
    {
        ExitRequested?.Invoke();
    }

    private void RequestSettings()
    {
        SettingsRequested?.Invoke();
    }
}