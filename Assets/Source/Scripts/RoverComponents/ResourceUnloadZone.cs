using System;
using UnityEngine;

public class ResourceUnloadZone : MonoBehaviour, IResourceReceiver
{
    [SerializeField] private Collider _zone;
    [SerializeField] private Transform _receivePoint;

    public Transform ReceivePoint => _receivePoint;

    public event Action<int> Unloaded;

    private void Awake()
    {
        _zone.isTrigger = true;
    }

    public bool CanReceive(IStorage storage)
    {
        return storage.CurrentResourcesMass > 0f;
    }

    public bool Receive(IStorage storage)
    {
        if (CanReceive(storage) == false)
            return false;

        Unloaded?.Invoke(storage.CurrentResourcesValue);

        storage.Clear();

        return true;
    }
}
