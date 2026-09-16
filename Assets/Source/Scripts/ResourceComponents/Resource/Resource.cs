using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Resource : MonoBehaviour, IAttractable
{
    [SerializeField] private GameObject[] _visualVariants;
    [SerializeField] private float _attractionSpeed = 10f;

    private Rigidbody _rigidbody;
    private ResourceConfig _config;
    private Transform _target;

    public ResourceConfig Config => _config;
    public event Action<Resource> ResourceConsumed;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_target == null)
            return;

        Vector3 direction = _target.position - transform.position;

        _rigidbody.velocity = direction.normalized * _attractionSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_config == null)
        {
            Debug.Log("config is null.");
            return;
        }

        if (other.TryGetComponent(out IStorage storage))
        {
            if (storage.TryAdd(_config))
            {
                Consume();
                Debug.Log("Sent to container");
            }
            else
                _target = null;
        }
    }

    public void Initialize(ResourceConfig config)
    {
        _config = config;

        int index = UnityEngine.Random.Range(0, _visualVariants.Length);

        for (int i = 0; i < _visualVariants.Length; i++)
            _visualVariants[i].SetActive(i == index);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void Consume()
    {
        ResourceConsumed?.Invoke(this);
    }

    public void ResetState()
    {
        _config = null;
        _target = null;

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        for (int i = 0; i < _visualVariants.Length; i++)
            _visualVariants[i].SetActive(false);
    }
}