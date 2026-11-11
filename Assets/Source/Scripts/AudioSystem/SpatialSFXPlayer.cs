using System.Collections.Generic;
using UnityEngine;

public class SpatialSFXPlayer : MonoBehaviour
{
    [SerializeField] private AudioEmitterSpawner _audioEmitterSpawner;
    [SerializeField] private SFX[] _sounds;
    [SerializeField] private SFXSettings _standartPlaybackSettings;
    [SerializeField] private int _maxSoundsCount;
    [SerializeField, Min(0f)] private float _minSpawnInterval = 0.08f;

    private int _soundsCount;

    private readonly Dictionary<SFXID, float> _nextAllowedTime = new();

    public void PlayEffectAtPoint(Vector3 position, SFXID id, SFXSettings settings = null)
    {
        if (_soundsCount >= _maxSoundsCount)
            return;

        AudioClip clip = null;

        foreach (var sound in _sounds)
        {
            if (sound.ID != id)
                continue;

            clip = sound.Clip;
            break;
        }

        if (clip == null)
        {
            Debug.LogWarning($"SFX not found or clip missing: {id}");
            return;
        }

        float currentTime = Time.unscaledTime;

        if (_nextAllowedTime.TryGetValue(id, out float nextTime) && currentTime < nextTime)
        {
            return;
        }

        AudioEmitter effect = _audioEmitterSpawner.Spawn(position);

        effect.ReadyToReturn += OnReadyToReturn;

        SFXSettings settingsToApply = settings == null ? _standartSettings : settings;
        effect.ApplySettings(settingsToApply);

        effect.Play(clip);

        _soundsCount++;
        _nextAllowedTime[id] = currentTime + _minSpawnInterval;
    }

    private void OnReadyToReturn(AudioEmitter effect)
    {
        effect.ReadyToReturn -= OnReadyToReturn;

        _audioEmitterSpawner.Despawn(effect);
        _soundsCount--;
    }
}
