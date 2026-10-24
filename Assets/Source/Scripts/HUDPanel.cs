using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDPanel : UIPanel
{
    [SerializeField] private Button _pauseButton;

    public event Action PauseRequested;

    private void OnEnable()
    {
        _pauseButton.onClick?.AddListener(RequestPause);
    }

    private void OnDisable()
    {
        _pauseButton?.onClick?.RemoveListener(RequestPause);
    }

    private void RequestPause()
    {
        PauseRequested?.Invoke();
    }
}