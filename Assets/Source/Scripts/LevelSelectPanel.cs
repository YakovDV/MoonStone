using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectPanel : UIPanel
{
    [SerializeField] private Button _exitButton;
    public event Action ReturnRequested;

    private void OnEnable()
    {
        _exitButton.onClick?.AddListener(RequestReturn);
    }

    private void OnDisable()
    {
        _exitButton.onClick?.RemoveListener(RequestReturn);
    }

    private void RequestReturn()
    {
        ReturnRequested?.Invoke();
    }
}
