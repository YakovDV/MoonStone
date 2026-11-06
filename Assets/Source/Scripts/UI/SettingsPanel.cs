using System;
using UnityEngine;
using UnityEngine.UI;

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
