using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class UniversalPool<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private T _prefab;
    [SerializeField] private int _poolCapacity = 120;
    [SerializeField] private int _poolMaxSize = 120;

    private ObjectPool<T> _pool;
    private HashSet<T> _objects = new();

    private readonly List<T> _activeSnapshotBuffer = new List<T>();

    public int PoolCapacity => _poolCapacity;
    public int CountAll => _pool.CountAll;

    private void Awake()
    {
        _pool = new ObjectPool<T>(
            createFunc: () => Instantiate(_prefab),
            actionOnGet: (@object) => @object.gameObject.SetActive(true),
            actionOnRelease: (@object) => @object.gameObject.SetActive(false),
            actionOnDestroy: (@object) => Destroy(@object.gameObject),
            collectionCheck: true,
            defaultCapacity: _poolCapacity,
            maxSize: _poolMaxSize
            );
    }

    public T GetObject()
    {
        T @object = _pool.Get();
        _objects.Add(@object);
        return @object;
    }

    public void ReleaseObject(T @object)
    {
        if (@object == null)
            return;

        if (_objects.Remove(@object) == false)
            return;

        _pool.Release(@object);
    }

    public void ReleaseAllObjects()
    {
        foreach (var t in _objects)
        {
            _pool.Release(t);
        }

        _objects.Clear();
    }

    public void ForEachActive(System.Action<T> action)
    {
        _activeSnapshotBuffer.Clear();

        foreach (var obj in _objects)
        {
            _activeSnapshotBuffer.Add(obj);
        }

        for (int i = 0; i < _activeSnapshotBuffer.Count; i++)
        {
            action(_activeSnapshotBuffer[i]);
        }
    }
}