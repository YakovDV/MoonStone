using UnityEngine;

[RequireComponent(typeof(Resource))]
public class ResourceEffector : MonoBehaviour
{
    [SerializeField] private SFXID _sfxID;
    [SerializeField] private SFXSettings _sfxSettings;

    private SpatialSFXPlayer _sfxPlayer;
    private Resource _resource;

    private void Awake()
    {
        _resource = GetComponent<Resource>();
        _resource.ResourceConsumed += OnResourceConsumed;
    }

    private void OnDestroy()
    {
        _resource.ResourceConsumed -= OnResourceConsumed;
    }

    public void Initialize(SpatialSFXPlayer sfxPlayer)
    {
        _sfxPlayer = sfxPlayer;
    }

    private void OnResourceConsumed(Resource resource)
    {
        _sfxPlayer.PlayEffectAtPoint(resource.transform.position, _sfxID, _sfxSettings);
    }
}