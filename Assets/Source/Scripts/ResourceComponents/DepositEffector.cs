using UnityEngine;

public class DepositEffector : MonoBehaviour
{
    [SerializeField] private DepositSpawnDispatcher _dispatcher;
    [SerializeField] private ParticleEffectPlayer _particleEffectPlayer;
    [SerializeField] private SpatialSFXPlayer _spatialSFXPlayer;
    [SerializeField] private SFXID sfxID;

    private void OnEnable()
    {
        _dispatcher.DepositDestroyedAtPoint += OnDepositDestroyedAtPoint;
    }

    private void OnDisable()
    {
        _dispatcher.DepositDestroyedAtPoint -= OnDepositDestroyedAtPoint;
    }

    private void OnDepositDestroyedAtPoint(Vector3 effectPosition)
    {
        _particleEffectPlayer.PlayEffectAtPoint(effectPosition);
        _spatialSFXPlayer.PlayEffectAtPoint(effectPosition, sfxID);
    }
}