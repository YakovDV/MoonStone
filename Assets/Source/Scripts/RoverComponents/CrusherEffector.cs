using UnityEngine;

public sealed class CrusherEffector : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _effects;

    private ParticleSystem _current;

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
    }

    public void Stop()
    {
        if (_current == null)
            return;

        _current.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        _current = null;
    }
}