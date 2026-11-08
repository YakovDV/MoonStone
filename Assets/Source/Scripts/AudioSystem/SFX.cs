using System;
using UnityEngine;

[Serializable]
public class SFX
{
    [SerializeField] private SFXID _id;
    [SerializeField] private AudioClip _clip;

    public SFXID ID => _id;
    public AudioClip Clip => _clip;
}
