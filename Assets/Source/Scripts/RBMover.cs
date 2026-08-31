using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class RBMover : MonoBehaviour
{
    [SerializeField] private PlayerGroundDetector _groundDetector;
    [SerializeField] private PlayerInputReader _inputReader;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _rotationSpeed = 180f;

    private Rigidbody _rigidbody;
    private Collider _collider;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    private void Update()
    {
        Move(_inputReader.Move.normalized);
    }

    private void Move(Vector2 input)
    {
        if (input.sqrMagnitude <= 0.00001f)
            return;

        Vector3 direction = new(input.x, 0f, input.y);

        _rigidbody.velocity = direction * _speed;

        Rotate(direction);
    }

    private void Rotate(Vector3 direction)
    {
        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }
}
