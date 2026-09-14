using System;
using UnityEngine;

public class UniversalSpawner<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private UniversalPool<T> _objectPool;

    public event Action ObjectSpawned;
    public event Action ObjectReturned;

    protected UniversalPool<T> ObjectPool => _objectPool;

    public T Spawn(Vector3 spawnPoint)
    {
        return ActivateObject(spawnPoint);
    }

    public void Despawn(T @object)
    {
        ReturnObject(@object);
    }

    protected T ActivateObject(Vector3 spawnPoint)
    {
        T @object = _objectPool.GetObject();

        ObjectSpawned?.Invoke();

        @object.transform.SetPositionAndRotation(spawnPoint, Quaternion.identity);

        return @object;
    }

    protected void ReturnObject(T @object)
    {
        ObjectReturned?.Invoke();
        _objectPool.ReleaseObject(@object);
    }
}