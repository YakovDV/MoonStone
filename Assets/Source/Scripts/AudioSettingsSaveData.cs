using System;
using UnityEngine;

[Serializable]
public class AudioSettingsSaveData
{
    [SerializeField] private float _masterVolume = 0.8f;
    [SerializeField] private float _musicVolume = 0.8f;
    [SerializeField] private float _sfxVolume = 0.8f;

    public float MasterVolume => _masterVolume;
    public float MusicVolume => _musicVolume;
    public float SfxVolume => _sfxVolume;

    public AudioSettingsSaveData(float masterVolume, float musicVolume, float sfxVolume)
    {
        _masterVolume = masterVolume;
        _musicVolume = musicVolume;
        _sfxVolume = sfxVolume;
    }
}