using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompletePanel : UIPanel
{
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private TextMeshProUGUI _bonus;
    [SerializeField] private string _bonusPreText;
    [SerializeField] private TextMeshProUGUI _totalProgress;
    [SerializeField] private string _totalProgressText;

    public event Action RestartRequested;
    public event Action ExitRequested;
    public event Action NextRequested;

    private void OnEnable()
    {
        _restartButton.onClick?.AddListener(RequestRestart);
        _exitButton.onClick?.AddListener(RequestExit);
        _nextButton.onClick?.AddListener(RequestNext);
    }

    private void OnDisable()
    {
        _restartButton.onClick?.RemoveListener(RequestRestart);
        _exitButton.onClick?.RemoveListener(RequestExit);
        _nextButton.onClick?.RemoveListener(RequestNext);
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

    private void RequestNext()
    {
        NextRequested?.Invoke();
    }
}