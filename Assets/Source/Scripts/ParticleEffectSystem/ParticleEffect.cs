using System;
using System.Collections;
using UnityEngine;

public class ParticleEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;

    private Coroutine _playCoroutine;
    private WaitForSeconds _returnDelay;

    public event Action<ParticleEffect> ReadyToReturn;

    private void Awake()
    {
        _returnDelay = new(_particleSystem.main.duration);
    }

    public void Play()
    {
        if (_playCoroutine != null)
        {
            StopCoroutine(_playCoroutine);
            _playCoroutine = null;
        }

        _playCoroutine = StartCoroutine(ReturnAfterEffect());
    }

    private IEnumerator ReturnAfterEffect()
    {
        _particleSystem.Play();

        yield return _returnDelay;

        ReadyToReturn?.Invoke(this);
    }
}
