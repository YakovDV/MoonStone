using UnityEngine;

public class MoneyEffector : MonoBehaviour
{
    [SerializeField] private ParticleEffectPlayer _particleEffectPlayer;
    [SerializeField] private SpatialSFXPlayer _spatialSFXPlayer;
    [SerializeField] private SFXID sfxID;

    public void PlayEffectAtPoint(Vector3 effectPosition)
    {
        _particleEffectPlayer.PlayEffectAtPoint(effectPosition);
        _spatialSFXPlayer.PlayEffectAtPoint(effectPosition, sfxID);
    }
}