using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private SFX[] _sounds;

    public void Play(SFXID id)
    {
        foreach (var sound in _sounds)
        {
            if (sound.ID != id)
                continue;

            if (sound.Clip != null)
                _audioSource.PlayOneShot(sound.Clip);

            return;
        }

        Debug.LogWarning($"SFX not found: {id}");
    }
}
