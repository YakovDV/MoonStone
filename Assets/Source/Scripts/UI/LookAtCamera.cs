using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private void Awake()
    {
        if (_camera == null)
            _camera = Camera.main;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward, _camera.transform.rotation * Vector3.up);
    }
}