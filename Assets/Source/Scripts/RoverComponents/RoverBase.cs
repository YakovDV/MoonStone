using UnityEngine;

public class RoverBase : MonoBehaviour
{
    [SerializeField] private ResourceUnloadZone _resourceUnloadZone;
    [SerializeField] private Wallet _wallet;

    private void OnEnable()
    {
        _resourceUnloadZone.Unloaded += OnUnloaded;
    }

    private void OnDisable()
    {
        _resourceUnloadZone.Unloaded -= OnUnloaded;
    }

    public void Initialize()
    {
        
    }

    private void OnUnloaded(int value)
    {
        _wallet.Add(value);
    }
}
