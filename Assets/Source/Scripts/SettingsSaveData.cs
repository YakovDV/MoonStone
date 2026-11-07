using System;
using UnityEngine;

[Serializable]
public class SettingsSaveData
{
    [SerializeField] private AudioSettingsSaveData _audioSettings;
    //public LanguageSettingsSaveData Language = new();

    public AudioSettingsSaveData AudioSettingsSaveData => _audioSettings;

    public SettingsSaveData(AudioSettingsSaveData audioSettingsSaveData)
    {
        _audioSettings = audioSettingsSaveData;
    }

    public void SetAudioSettings(AudioSettingsSaveData data)
    {
        _audioSettings = data ?? throw new ArgumentNullException(nameof(data));
    }
}
