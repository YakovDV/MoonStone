using UnityEngine;

public class SpatialSFXPlayer : MonoBehaviour
{
    [SerializeField] private AudioEmitterSpawner _audioEmitterSpawner;
    [SerializeField] private SFX[] _sounds;

    public void PlayEffectAtPoint(Vector3 position, SFXID id)
    {
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

        AudioEmitter effect = _audioEmitterSpawner.Spawn(position);

        effect.ReadyToReturn += OnReadyToReturn;

        effect.Play(clip);
    }

    private void OnReadyToReturn(AudioEmitter effect)
    {
        effect.ReadyToReturn -= OnReadyToReturn;
        _audioEmitterSpawner.Despawn(effect);
    }
}
