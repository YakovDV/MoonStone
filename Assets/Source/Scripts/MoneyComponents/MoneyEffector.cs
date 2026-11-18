using UnityEngine;
using Unity.Profiling;


public class MoneyEffector : MonoBehaviour
{
    [SerializeField] private ParticleEffectPlayer _particleEffectPlayer;

    [Header("Audio effect")]
    [SerializeField] private SpatialSFXPlayer _spatialSFXPlayer;
    [SerializeField] private SFXID sfxID;
    [SerializeField] private SFXSettings _sfxSettings;

    private static readonly ProfilerMarker ParticleMarker =
    new("Money.ParticleEffect");

    private static readonly ProfilerMarker AudioMarker =
        new("Money.AudioEffect");

    public void PlayEffectAtPoint(Vector3 effectPosition)
    {
        _particleEffectPlayer.PlayEffectAtPoint(effectPosition);
        _spatialSFXPlayer.PlayEffectAtPoint(effectPosition, sfxID, _sfxSettings);

        using (ParticleMarker.Auto())
            _particleEffectPlayer.PlayEffectAtPoint(effectPosition);

        using (AudioMarker.Auto())
            _spatialSFXPlayer.PlayEffectAtPoint(effectPosition, sfxID, _sfxSettings);
    }
}