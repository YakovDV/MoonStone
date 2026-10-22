using System.Collections;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _trackingPoint;

    [SerializeField] private Vector3 _cameraOffset;
    [SerializeField] private Vector3 _cameraAngle;

    [SerializeField] private Vector3 _alternativeOffset;
    [SerializeField] private Vector3 _alternativeAngle;

    [SerializeField] private float _changePositionDiration = 2;

    private Vector3 _currentOffset;

    private Coroutine _changePositionCoroutine;

    private void OnEnable()
    {
        ResetCamera();
    }

    private void LateUpdate()
    {
        _camera.transform.position = _trackingPoint.position + _currentOffset;
    }

    public void ChangePositionSmooth(CameraPositionType cameraPositionType)
    {
        if (_changePositionCoroutine != null)
        {
            StopCoroutine(_changePositionCoroutine);
            _changePositionCoroutine = null;
        }

        switch (cameraPositionType)
        {
            case CameraPositionType.NormalPosition:
                _changePositionCoroutine = StartCoroutine(MoveCamera(_cameraOffset, _cameraAngle));
                break;
            case CameraPositionType.AlternativePosition:
                _changePositionCoroutine = StartCoroutine(MoveCamera(_alternativeOffset, _alternativeAngle));
                break;
        }
    }

    public void ResetCamera()
    {
        if (_changePositionCoroutine != null)
        {
            StopCoroutine(_changePositionCoroutine);
            _changePositionCoroutine = null;
        }

        _currentOffset = _cameraOffset;
        _camera.transform.rotation = Quaternion.Euler(_cameraAngle);
    }

    private IEnumerator MoveCamera(Vector3 targetPositionOffset, Vector3 targetRotation)
    {
        float elapsed = 0f;

        Vector3 startOffset = _currentOffset;
        Vector3 startRotation = _camera.transform.rotation.eulerAngles;

        while (elapsed < _changePositionDiration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / _changePositionDiration);
            t = Mathf.SmoothStep(0f, 1f, t);

            _currentOffset = Vector3.Lerp(startOffset, targetPositionOffset, t);
            Vector3 rotation = Vector3.Lerp(startRotation, targetRotation, t);

            _camera.transform.rotation = Quaternion.Euler(rotation);

            yield return null;
        }

        _currentOffset = targetPositionOffset;
        _camera.transform.rotation = Quaternion.Euler(targetRotation);
    }
}

public enum CameraPositionType
{
    NormalPosition,
    AlternativePosition
}