using System;
using System.Collections;
using UnityEngine;

public class AudioEmitter : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    private Coroutine _playCoroutine;

    public event Action<AudioEmitter> ReadyToReturn;

    public void Play(AudioClip clip)
    {
        if (_playCoroutine != null)
        {
            StopCoroutine(_playCoroutine);
            _playCoroutine = null;
        }

        _audioSource.clip = clip;

        _playCoroutine = StartCoroutine(ReturnAfterEffect());
    }

    private IEnumerator ReturnAfterEffect()
    {
        _audioSource.Play();

        yield return null;

        while (_audioSource.isPlaying)
            yield return null;

        _playCoroutine = null;
        ReadyToReturn?.Invoke(this);
    }
}