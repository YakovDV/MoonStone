using System;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : UIPanel
{
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _exitButton;

    public event Action ResumeRequested;
    public event Action ExitRequested;

    private void OnEnable()
    {
        _resumeButton.onClick?.AddListener(RequestResume);
        _exitButton.onClick?.AddListener(RequestExit);
    }

    private void RequestResume()
    {
        ResumeRequested?.Invoke();
    }

    private void RequestExit()
    {
        ExitRequested?.Invoke();
    }
}