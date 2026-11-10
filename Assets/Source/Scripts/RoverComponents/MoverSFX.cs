using System;
using System.Collections;
using UnityEngine;

public class MoverSFX : MonoBehaviour
{
    [SerializeField] private PlayerMover _playerMover;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _fadeTime = 0.5f;

    private bool _wasMoving;

    private Coroutine VolumeCoroutine;

    private void Update()
    {
        bool isMoving = _playerMover.Speed > 0f;

        if (isMoving == _wasMoving)
            return;

        _wasMoving = isMoving;

        if (VolumeCoroutine != null)
        {
            StopCoroutine(VolumeCoroutine);
            VolumeCoroutine = null;
        }

        if (isMoving)
        {
            if (_audioSource.isPlaying == false)
            {
                _audioSource.volume = 0f;
                _audioSource.Play();
            }

            VolumeCoroutine = StartCoroutine(FadeVolume(_audioSource.volume, 1f, null));
        }
        else
        {
            VolumeCoroutine = StartCoroutine(FadeVolume(_audioSource.volume, 0f, _audioSource.Stop));
        }
    }

    private IEnumerator FadeVolume(float startVolume, float targetVolume, Action action)
    {
        _audioSource.volume = startVolume;

        float elapsed = 0f;

        while (elapsed < _fadeTime)
        {
            elapsed += Time.unscaledDeltaTime;

            _audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / _fadeTime);

            yield return null;
        }

        action?.Invoke();

        _audioSource.volume = targetVolume;
    }
}