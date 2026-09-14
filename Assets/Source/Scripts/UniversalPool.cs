using UnityEngine;
using UnityEngine.Pool;

public class UniversalPool<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private T _prefab;
    [SerializeField] private int _poolCapacity = 120;
    [SerializeField] private int _poolMaxSize = 120;

    private ObjectPool<T> _pool;

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
        return _pool.Get();
    }

    public void ReleaseObject(T @object)
    {
        _pool.Release(@object);
    }
}