using UnityEngine;

public sealed class PlayerCollisionSolver : MonoBehaviour
{
    private const float MinimumMagnitude = 0.000001f;

    [SerializeField] private BoxCollider _bodyCollider;
    [SerializeField] private LayerMask _collisionMask;

    [SerializeField, Min(0f)] private float _skinWidth = 0.02f;
    [SerializeField, Min(1)] private int _movementIterations = 3;
    [SerializeField, Min(1)] private int _depenetrationIterations = 3;
    [SerializeField, Min(0.1f)] private float _maximumRotationStep = 1.5f;

    [SerializeField] private bool _drawDebug = false;

    private readonly Vector3[] _corners = new Vector3[8];
    private readonly Collider[] _overlaps = new Collider[16];

    public CollisionPose ResolvePose(Vector3 position, Quaternion currentHeading, Quaternion desiredHeading, Vector3 displacement)
    {
        if (_drawDebug == true)
        {
            DrawCollisionBox(position, currentHeading, Color.green);
            DrawCollisionBox(position + displacement, desiredHeading, Color.yellow);
        }

        Vector3 resolvedPosition = position;
        Quaternion resolvedHeading = currentHeading;

        ResolveRotation(ref resolvedPosition, ref resolvedHeading, desiredHeading);

        Vector3 movementStartPosition = resolvedPosition;

        ResolveMovement(ref resolvedPosition, resolvedHeading, displacement);

        if (ResolvePenetration(ref resolvedPosition, resolvedHeading) == false)
        {
            resolvedPosition = movementStartPosition;
        }

        Vector3 resolvedMovement = resolvedPosition - movementStartPosition;

        if (_drawDebug == true)
            DrawCollisionBox(resolvedPosition, resolvedHeading, Color.cyan);

        return new CollisionPose(resolvedPosition, resolvedHeading, resolvedMovement);
    }

    private void ResolveRotation(ref Vector3 position, ref Quaternion heading, Quaternion desiredHeading)
    {
        float angle = Quaternion.Angle(heading, desiredHeading);

        if (angle < MinimumMagnitude)
            return;

        Quaternion initialHeading = heading;

        int steps = Mathf.Max(1, Mathf.CeilToInt(angle / _maximumRotationStep));

        for (int i = 1; i <= steps; i++)
        {
            Vector3 previousPosition = position;
            Quaternion previousHeading = heading;

            float t = i / (float)steps;

            heading = Quaternion.Slerp(initialHeading, desiredHeading, t);

            if (ResolvePenetration(ref position, heading))
            {
                continue;
            }

            position = previousPosition;
            heading = previousHeading;

            break;
        }
    }

    private void ResolveMovement(ref Vector3 position, Quaternion heading, Vector3 displacement)
    {
        Vector3 remaining = displacement;

        for (int i = 0; i < _movementIterations; i++)
        {
            Vector3 planarRemaining = Vector3.ProjectOnPlane(remaining, Vector3.up);

            float planarDistance = planarRemaining.magnitude;

            if (planarDistance < MinimumMagnitude)
            {
                position += remaining;
                return;
            }

            Vector3 direction = planarRemaining / planarDistance;
            Vector3 center = GetCenter(position, heading);

            if (_drawDebug == true)
            {
                Debug.DrawRay(center, direction * planarDistance, Color.white);
            }

            if (Physics.BoxCast(center, GetHalfExtents(), direction, out RaycastHit hit, heading, planarDistance + _skinWidth, _collisionMask, QueryTriggerInteraction.Ignore) == false)
            {
                position += remaining;
                return;
            }

            float allowedDistance = Mathf.Clamp(hit.distance - _skinWidth, 0f, planarDistance);
            float movementFactor = allowedDistance / planarDistance;

            Vector3 travelled = remaining * movementFactor;

            position += travelled;
            remaining -= travelled;

            if (_drawDebug == true)
            {
                DrawCollisionBox(position, heading, Color.red);

                Debug.DrawRay(hit.point, hit.normal * 0.5f, Color.magenta);
            }

            Vector3 obstacleNormal = Vector3.ProjectOnPlane(hit.normal, Vector3.up);

            if (obstacleNormal.sqrMagnitude <
                MinimumMagnitude)
            {
                return;
            }

            obstacleNormal.Normalize();

            remaining = Vector3.ProjectOnPlane(remaining, obstacleNormal);

            if (remaining.sqrMagnitude <
                MinimumMagnitude)
            {
                return;
            }
        }
    }

    private bool ResolvePenetration(ref Vector3 position, Quaternion heading)
    {
        for (int iteration = 0; iteration < _depenetrationIterations; iteration++)
        {
            int count = Physics.OverlapBoxNonAlloc(GetCenter(position, heading), GetHalfExtents(), _overlaps, heading, _collisionMask, QueryTriggerInteraction.Ignore);

            bool hasPenetration = false;

            for (int i = 0; i < count; i++)
            {
                Collider other = _overlaps[i];

                if (other == null || other == _bodyCollider)
                {
                    continue;
                }

                if (Physics.ComputePenetration(_bodyCollider, position, heading, other, other.transform.position, other.transform.rotation, out Vector3 direction, out float distance) == false)
                {
                    continue;
                }

                hasPenetration = true;

                if (_drawDebug == true)
                {
                    DrawCollisionBox(position, heading, Color.red);
                }

                Vector3 planarDirection = Vector3.ProjectOnPlane(direction, Vector3.up);

                float planarMagnitude = planarDirection.magnitude;

                if (planarMagnitude < MinimumMagnitude)
                {
                    return false;
                }

                planarDirection /= planarMagnitude;

                float correctionDistance = distance / planarMagnitude;

                Vector3 correction = planarDirection * (correctionDistance + _skinWidth);

                if (_drawDebug == true)
                {
                    Debug.DrawRay(GetCenter(position, heading), correction, Color.magenta);
                }

                position += correction;
            }

            if (hasPenetration == false)
                return true;
        }

        return HasPenetration(position, heading) == false;
    }

    private bool HasPenetration(Vector3 position, Quaternion heading)
    {
        int count = Physics.OverlapBoxNonAlloc(GetCenter(position, heading), GetHalfExtents(), _overlaps, heading, _collisionMask, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < count; i++)
        {
            Collider other = _overlaps[i];

            if (other == null || other == _bodyCollider)
            {
                continue;
            }

            if (Physics.ComputePenetration(_bodyCollider, position, heading, other, other.transform.position, other.transform.rotation, out _, out _))
            {
                return true;
            }
        }

        return false;
    }

    private Vector3 GetCenter(Vector3 position, Quaternion heading)
    {
        Vector3 center = Vector3.Scale(_bodyCollider.center, GetAbsoluteScale());

        return position + heading * center;
    }

    private Vector3 GetHalfExtents()
    {
        Vector3 size = Vector3.Scale(_bodyCollider.size, GetAbsoluteScale());

        return size * 0.5f;
    }

    private Vector3 GetAbsoluteScale()
    {
        Vector3 scale = transform.lossyScale;

        return new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
    }

    private void DrawCollisionBox(Vector3 position, Quaternion heading, Color color)
    {
        DrawBox(GetCenter(position, heading), GetHalfExtents(), heading, color);
    }

    private void DrawBox(Vector3 center, Vector3 halfExtents, Quaternion rotation, Color color)
    {
        _corners[0] = new(-halfExtents.x, -halfExtents.y, -halfExtents.z);
        _corners[1] = new(halfExtents.x, -halfExtents.y, -halfExtents.z);
        _corners[2] = new(halfExtents.x, -halfExtents.y, halfExtents.z);
        _corners[3] = new(-halfExtents.x, -halfExtents.y, halfExtents.z);

        _corners[4] = new(-halfExtents.x, halfExtents.y, -halfExtents.z);
        _corners[5] = new(halfExtents.x, halfExtents.y, -halfExtents.z);
        _corners[6] = new(halfExtents.x, halfExtents.y, halfExtents.z);
        _corners[7] = new(-halfExtents.x, halfExtents.y, halfExtents.z);

        for (int i = 0; i < _corners.Length; i++)
            _corners[i] = center + rotation * _corners[i];

        Debug.DrawLine(_corners[0], _corners[1], color);
        Debug.DrawLine(_corners[1], _corners[2], color);
        Debug.DrawLine(_corners[2], _corners[3], color);
        Debug.DrawLine(_corners[3], _corners[0], color);

        Debug.DrawLine(_corners[4], _corners[5], color);
        Debug.DrawLine(_corners[5], _corners[6], color);
        Debug.DrawLine(_corners[6], _corners[7], color);
        Debug.DrawLine(_corners[7], _corners[4], color);

        Debug.DrawLine(_corners[0], _corners[4], color);
        Debug.DrawLine(_corners[1], _corners[5], color);
        Debug.DrawLine(_corners[2], _corners[6], color);
        Debug.DrawLine(_corners[3], _corners[7], color);
    }
}

public readonly struct CollisionPose
{
    public Vector3 Position { get; }
    public Quaternion Heading { get; }
    public Vector3 Movement { get; }

    public CollisionPose(Vector3 position, Quaternion heading, Vector3 movement)
    {
        Position = position;
        Heading = heading;
        Movement = movement;
    }
}