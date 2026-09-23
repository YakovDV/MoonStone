using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Money : MonoBehaviour, IAttractable
{
    [SerializeField] private float _attractionSpeed = 10f;
    [SerializeField] private float _rotationSpeed = 180f;

    private int _worth;
    private Rigidbody _rigidbody;
    private Transform _target;

    public event Action<Money> Consumed;

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

        collector.Collect(_worth);
        Consume();
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void Initialize(int value)
    {
        _worth = value;
    }

    public void Consume()
    {
        Consumed?.Invoke(this);
    }

    public void ResetState()
    {
        _target = null;

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}