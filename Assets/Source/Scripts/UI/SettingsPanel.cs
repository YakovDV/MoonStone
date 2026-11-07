using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SettingsPanel : UIPanel
{
    [SerializeField] private Button _returnButton;

    [Inject] private SaveService _saveService;
    [Inject] private AudioVolumeSettings _volumeSettings;

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
        _saveService.SaveAudioSettings(_volumeSettings.GetSaveData());

        ReturnRequested?.Invoke();
    }
}
