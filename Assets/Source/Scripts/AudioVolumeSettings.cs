using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioVolumeSettings
{
    private const float DecibelMultiplier = 20f;

    private const string MasterVolumeParameter = "MasterVolume";
    private const string MusicVolumeParameter = "MusicVolume";
    private const string SfxVolumeParameter = "SFXVolume";

    private AudioMixer _audioMixer;

    private readonly Dictionary<string, float> _volumes = new();

    public AudioVolumeSettings(AudioMixer audioMixer)
    {
        _audioMixer = audioMixer;
    }

    public void Initialize(AudioSettingsSaveData data)
    {
        _volumes.Clear();

        _volumes[MasterVolumeParameter] = data.MasterVolume;
        _volumes[MusicVolumeParameter] = data.MusicVolume;
        _volumes[SfxVolumeParameter] = data.SfxVolume;

        foreach (var volume in _volumes)
            ApplyVolume(volume.Key, volume.Value);
    }

    public void SetVolume(string parameterName, float value)
    {
        ApplyVolume(parameterName, value);

        _volumes[parameterName] = value;
    }

    public float GetVolume(string parameterName)
    {
        return _volumes[parameterName];
    }

    public AudioSettingsSaveData GetSaveData()
    {
        return new(_volumes[MasterVolumeParameter], _volumes[MusicVolumeParameter], _volumes[SfxVolumeParameter]);
    }

    private void ApplyVolume(string parameterName, float value)
    {
        if (value <= 0)
        {
            _audioMixer.SetFloat(parameterName, -80f);
            return;
        }

        _audioMixer.SetFloat(parameterName, Mathf.Log10(value) * DecibelMultiplier);
    }
}
