using System;
using UnityEngine;
using UnityEngine.UI;

public class CompletePanel : UIPanel
{
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _exitButton;

    public event Action RestartRequested;
    public event Action ExitRequested;

    private void OnEnable()
    {
        _restartButton.onClick?.AddListener(RequestRestart);
        _exitButton.onClick?.AddListener(RequestExit);
    }

    private void OnDisable()
    {
        _restartButton.onClick?.RemoveListener(RequestRestart);
        _exitButton.onClick?.RemoveListener(RequestExit);
    }

    private void RequestRestart()
    {
        RestartRequested?.Invoke();
    }

    private void RequestExit()
    {
        RestartRequested?.Invoke();
    }
}