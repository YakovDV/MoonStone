using System;
using UnityEngine;

public class ParticleEffectPlayer : MonoBehaviour
{
    [SerializeField] private ParticleEffectSpawner _spawner;

    private Action<ParticleEffect> _onReadyToReturn;

    private void Awake()
    {
        _onReadyToReturn = OnReadyToReturn;
    }

    public void PlayEffectAtPoint(Vector3 position)
    {
        ParticleEffect effect = _spawner.Spawn(position);

        effect.ReadyToReturn += _onReadyToReturn;

        effect.Play();
    }

    private void OnReadyToReturn(ParticleEffect effect)
    {
        effect.ReadyToReturn -= _onReadyToReturn;
        _spawner.Despawn(effect);
    }
}
