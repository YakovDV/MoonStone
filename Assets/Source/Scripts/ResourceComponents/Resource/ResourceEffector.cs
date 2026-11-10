using UnityEngine;

[RequireComponent(typeof(Resource))]
public class ResourceEffector : MonoBehaviour
{
    [SerializeField] private SFXID _sfxID;

    private SpatialSFXPlayer _sfxPlayer;
    private Resource _resource;

    private void Awake()
    {
        _resource = GetComponent<Resource>();
    }

    private void OnEnable()
    {
        _resource.ResourceConsumed += OnResourceConsumed;
    }

    private void OnDisable()
    {
        _resource.ResourceConsumed -= OnResourceConsumed;
    }

    public void Initialize(SpatialSFXPlayer sfxPlayer)
    {
        _sfxPlayer = sfxPlayer;
    }

    private void OnResourceConsumed(Resource resource)
    {
        _sfxPlayer.PlayEffectAtPoint(resource.transform.position, _sfxID);
    }
}