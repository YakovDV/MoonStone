using UnityEngine;

public class MoneyEffector : MonoBehaviour
{
    [SerializeField] private ParticleEffectPlayer _particleEffectPlayer;

    [Header("Audio effect")]
    [SerializeField] private SpatialSFXPlayer _spatialSFXPlayer;
    [SerializeField] private SFXID sfxID;
    [SerializeField] private SFXSettings _sfxSettings;

    public void PlayEffectAtPoint(Vector3 effectPosition)
    {
        _particleEffectPlayer.PlayEffectAtPoint(effectPosition);
        _spatialSFXPlayer.PlayEffectAtPoint(effectPosition, sfxID, _sfxSettings);
    }
}