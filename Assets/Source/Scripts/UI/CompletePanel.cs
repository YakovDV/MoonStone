using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompletePanel : UIPanel
{
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private TextMeshProUGUI _bonus;
    [SerializeField] private string _bonusPreText;
    [SerializeField] private TextMeshProUGUI _totalProgress;
    [SerializeField] private string _totalProgressText;

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

    public void ShowResult(int bonus, int totalProgress)
    {
        _bonus.text = _bonusPreText + bonus.ToString();
        _totalProgress.text = _totalProgressText + totalProgress.ToString();
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