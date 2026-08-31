using UnityEngine;

public sealed class PlayerGroundDetector : MonoBehaviour
{
    private const int RaysCount = 4;
    private const float MinimumMagnitude = 0.000001f;

    [SerializeField] private Vector3[] _rayOrigins;
    [SerializeField] private float _rayStartHeight = 4f;
    [SerializeField] private float _rayDistance = 8f;
    [SerializeField] private LayerMask _groundMask;

    private readonly Vector3[] _hitPoints = new Vector3[RaysCount];

    public GroundInfo Detect(Vector3 position, Quaternion rotation, Vector3 direction)
    {
        if (_rayOrigins == null || _rayOrigins.Length != RaysCount)
            return default;

        if (direction.sqrMagnitude < MinimumMagnitude)
            return default;

        direction.Normalize();

        Vector3 up = -direction;
        Vector3 planarForward = Vector3.ProjectOnPlane(rotation * Vector3.forward, up);

        if (planarForward.sqrMagnitude < MinimumMagnitude)
            return default;

        Quaternion planarRotation = Quaternion.LookRotation(planarForward.normalized, up);

        Vector3 bodyNormal = rotation * Vector3.up;

        float verticalAlignment = Vector3.Dot(bodyNormal, up);

        if (Mathf.Abs(verticalAlignment) < MinimumMagnitude)
            return default;

        float offset = 0f;

        for (int i = 0; i < _rayOrigins.Length; i++)
        {
            Vector3 planarOffset = planarRotation * _rayOrigins[i];
            Vector3 lateralOffset = Vector3.ProjectOnPlane(planarOffset, up);

            float probeHeight = (_rayOrigins[i].y - Vector3.Dot(bodyNormal, lateralOffset)) / verticalAlignment;

            Vector3 probePoint = position + lateralOffset + up * probeHeight;
            Vector3 rayOrigin = probePoint + up * _rayStartHeight;

            Debug.DrawRay(rayOrigin, direction * _rayDistance, Color.red);

            if (Physics.Raycast(rayOrigin, direction, out RaycastHit hit, _rayDistance, _groundMask, QueryTriggerInteraction.Ignore) == false)
            {
                return default;
            }

            _hitPoints[i] = hit.point;

            offset += Vector3.Dot(hit.point - probePoint, direction);
        }

        Vector3 front = (_hitPoints[0] + _hitPoints[1]) * 0.5f;
        Vector3 rear = (_hitPoints[2] + _hitPoints[3]) * 0.5f;
        Vector3 left = (_hitPoints[0] + _hitPoints[2]) * 0.5f;
        Vector3 right = (_hitPoints[1] + _hitPoints[3]) * 0.5f;

        Vector3 surfaceForward = front - rear;
        Vector3 surfaceRight = right - left;

        Vector3 normal = Vector3.Cross(surfaceForward, surfaceRight);

        if (normal.sqrMagnitude < MinimumMagnitude)
            return default;

        normal.Normalize();

        if (Vector3.Dot(normal, up) < 0f)
            normal = -normal;

        Vector3 point = (_hitPoints[0] + _hitPoints[1] + _hitPoints[2] + _hitPoints[3]) / RaysCount;

        return new GroundInfo(true, normal, point, offset / RaysCount);
    }
}

public struct GroundInfo
{
    public bool IsGrounded;
    public Vector3 Normal;
    public Vector3 Point;
    public float Offset;

    public GroundInfo(bool isGrounded, Vector3 normal, Vector3 point, float offset)
    {
        IsGrounded = isGrounded;
        Normal = normal;
        Point = point;
        Offset = offset;
    }
}