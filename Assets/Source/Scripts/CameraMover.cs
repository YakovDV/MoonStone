using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _trackingPoint;

    [SerializeField] private Vector3 _cameraOffset;
    [SerializeField] private Vector3 _cameraAngle;

    private void OnEnable()
    {
        _camera.transform.position = _cameraOffset;
        _camera.transform.rotation = Quaternion.Euler(_cameraAngle);
    }

    private void LateUpdate()
    {
        _camera.transform.position = _trackingPoint.position + _cameraOffset;
    }
}