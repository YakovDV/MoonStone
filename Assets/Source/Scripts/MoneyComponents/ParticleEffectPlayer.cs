using UnityEngine;

public class ParticleEffectPlayer : MonoBehaviour
{
    [SerializeField] private ParticleEffectSpawner _spawner;

    public void PlayEffectAtPoint(Vector3 position)
    {
        ParticleEffect effect = _spawner.Spawn(position);

        effect.ReadyToReturn += OnReadyToReturn;

        effect.Play();
    }

    private void OnReadyToReturn(ParticleEffect effect)
    {
        effect.ReadyToReturn -= OnReadyToReturn;
        _spawner.Despawn(effect);
    }
}