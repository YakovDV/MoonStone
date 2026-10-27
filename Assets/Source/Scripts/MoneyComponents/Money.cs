using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Money : MonoBehaviour, IAttractable
{
    [SerializeField] private float _attractionSpeed = 10f;
    [SerializeField] private float _rotationSpeed = 180f;

    private Rigidbody _rigidbody;
    private Transform _target;

    public int Worth {  get; private set; }

    public event Action<Money> Collected;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime, Space.World);
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
        if (other.TryGetComponent(out MoneyCollector collector) == false)
            return;

        collector.Collect(Worth);
        Collected?.Invoke(this);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void Initialize(int value)
    {
        Worth = value;
    }

    public void ResetState()
    {
        _target = null;

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }
}