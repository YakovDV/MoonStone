using System;
using System.Collections;
using UnityEngine;

public class ParticleEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;

    private Coroutine _playCoroutine;

    public event Action<ParticleEffect> ReadyToReturn;

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
        WaitForSeconds wait = new(_particleSystem.main.duration);

        _particleSystem.Play();

        yield return wait;

        ReadyToReturn?.Invoke(this);
    }
}
