using System;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] private GameObject[] _visualVariants;

    private Rigidbody _rigidbody;
    private ResourceConfig _config;

    public ResourceConfig Config => _config;
    public event Action<Resource> ReadyToReturn;

    public void Initialize(ResourceConfig config)
    {
        _config = config;

        int index = UnityEngine.Random.Range(0, _visualVariants.Length);

        for (int i = 0; i < _visualVariants.Length; i++)
            _visualVariants[i].SetActive(i == index);
    }

    public void Consume()
    {
        ReadyToReturn?.Invoke(this);
        ResetState();
    }

    private void ResetState()
    {
        _config = null;

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        for (int i = 0; i < _visualVariants.Length; i++)
            _visualVariants[i].SetActive(false);
    }
}