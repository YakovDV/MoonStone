using System;
using UnityEngine;

[Serializable]
public class SFXSettings
{
    [SerializeField, Range(0f, 1f)] private float _volume = 1f;
    [SerializeField] private float _pitch = 1f;
    [SerializeField] private bool _randomizePitch;
    [SerializeField] private Vector2 _pitchRange = new(0.9f, 1.1f);

    public float Volume => _volume;
    public float Pitch => _pitch;
    public bool RandomizePitch => _randomizePitch;
    public Vector2 PitchRange => _pitchRange;
}