using System;
using UnityEngine;

[Serializable]
public class Music
{
    [SerializeField] private MusicID _musicID;
    [SerializeField] private AudioClip _audioClip;

    public MusicID MusicID => _musicID;
    public AudioClip AudioClip => _audioClip;

    public Music(MusicID musicID, AudioClip audioClip)
    {
        _musicID = musicID;
        _audioClip = audioClip;
    }
}
