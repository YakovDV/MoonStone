using System;
using System.Collections;
using UnityEngine;

public sealed class CrusherEffector : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _effects;
    [SerializeField] private AudioSource _audioEffect;
    [SerializeField] private float _fadeTime = 0.5f;

    private ParticleSystem _current;
    private Coroutine VolumeCoroutine;

    public void Play(int tier)
    {
        int index = tier - 1;

        if (index < 0 || index >= _effects.Length)
        {
            Stop();
            return;
        }

        ParticleSystem effect = _effects[index];

        if (_current == effect)
            return;

        Stop();

        _current = effect;
        _current.Play();
        PlaySFX();
    }

    public void Stop()
    {
        if (_current == null)
            return;

        _current.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        _current = null;

        StopSFX();
    }

    private void PlaySFX()
    {
        if (VolumeCoroutine != null)
        {
            StopCoroutine(VolumeCoroutine);
            VolumeCoroutine = null;
        }

        if (_audioEffect.isPlaying == false)
        {
            _audioEffect.volume = 0f;
            _audioEffect.Play();
        }

        VolumeCoroutine = StartCoroutine(FadeVolume(_audioEffect.volume, 1f, null));
    }

    private void StopSFX()
    {
        if (VolumeCoroutine != null)
        {
            StopCoroutine(VolumeCoroutine);
            VolumeCoroutine = null;
        }

        VolumeCoroutine = StartCoroutine(FadeVolume(_audioEffect.volume, 0f, _audioEffect.Stop));
    }

    private IEnumerator FadeVolume(float startVolume, float targetVolume, Action action)
    {
        _audioEffect.volume = startVolume;

        float elapsed = 0f;

        while (elapsed < _fadeTime)
        {
            elapsed += Time.unscaledDeltaTime;

            _audioEffect.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / _fadeTime);

            yield return null;
        }

        action?.Invoke();

        _audioEffect.volume = targetVolume;
    }
}