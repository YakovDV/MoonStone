using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    private const float MinimumMagnitude = 0.000001f;

    [SerializeField] private PlayerInputReader _inputReader;
    [SerializeField] private PlayerGroundDetector _groundDetector;
    [SerializeField] private PlayerCollisionSolver _collisionSolver;

    [SerializeField] private float _speed = 8f;
    [SerializeField] private float _rotationSpeed = 180f;
    [SerializeField] private float _groundAlignmentSpeed = 180f;

    [SerializeField] private float _maximumClimbAngle = 35f;
    [SerializeField] private float _slopeSpeedMultiplier = 0.5f;
    [SerializeField] private float _slideGravity = 12f;
    [SerializeField] private float _maximumSlideSpeed = 20f;

    private Vector3 _inputDirection;
    private Quaternion _headingRotation;
    private Vector3 _slideVelocity;

    public Vector3 Velocity { get; private set; }
    public bool IsGrounded { get; private set; }
    public float Speed { get; private set; }

    private void Start()
    {
        _headingRotation = GetPlanarRotation(transform.rotation);

        GroundInfo ground = _groundDetector.Detect(transform.position, transform.rotation, Vector3.down);
        IsGrounded = ground.IsGrounded;
    }

    private void Update()
    {
        _inputDirection = _inputReader.Move;
    }

    private void FixedUpdate()
    {
        Move(_inputDirection, Time.fixedDeltaTime);
    }

    private void Move(Vector2 input, float deltaTime)
    {
        Speed = 0f;
        Velocity = Vector3.zero;

        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;

        GroundInfo currentGround = _groundDetector.Detect(currentPosition, currentRotation, Vector3.down);

        IsGrounded = currentGround.IsGrounded;

        if (currentGround.IsGrounded == false)
        {
            _slideVelocity = Vector3.zero;
            return;
        }

        Quaternion desiredHeading = GetDesiredHeading(input, deltaTime);

        Vector3 candidateSlideVelocity = GetSlideVelocity(_slideVelocity, currentGround.Normal, deltaTime);

        Vector3 driveVelocity = GetDriveVelocity(input, desiredHeading, currentGround.Normal);
        Vector3 desiredVelocity = driveVelocity + _slideVelocity;

        if (desiredVelocity.sqrMagnitude < MinimumMagnitude)
        {
            _slideVelocity = candidateSlideVelocity;

            ApplyRotationOnly(currentPosition, currentRotation, currentGround, desiredHeading, deltaTime);

            return;
        }

        CollisionPose collisionPose = _collisionSolver.ResolvePose(currentPosition, _headingRotation, desiredHeading, desiredVelocity * deltaTime);

        if (TryGetGroundPose(collisionPose.Position, currentRotation, collisionPose.Heading, currentGround, deltaTime, out Vector3 desiredPosition, out Quaternion desiredRotation, out GroundInfo desiredGround) == false)
        {
            ApplyRotationOnly(currentPosition, currentRotation, currentGround, desiredHeading, deltaTime);

            return;
        }

        Vector3 finalRequestedVelocity = desiredVelocity;
        Vector3 finalSlideVelocity = candidateSlideVelocity;

        if (IsSteep(desiredGround.Normal))
        {
            Vector3 slopeDriveVelocity = GetDriveVelocity(input, desiredHeading, desiredGround.Normal);
            Vector3 slopeSlideVelocity = Vector3.ProjectOnPlane(candidateSlideVelocity, desiredGround.Normal);

            slopeSlideVelocity = Vector3.ClampMagnitude(slopeSlideVelocity, _maximumSlideSpeed);

            Vector3 slopeVelocity = slopeDriveVelocity + slopeSlideVelocity;

            if ((slopeVelocity - desiredVelocity).sqrMagnitude > MinimumMagnitude)
            {
                collisionPose = _collisionSolver.ResolvePose(currentPosition, _headingRotation, desiredHeading, slopeVelocity * deltaTime);

                if (TryGetGroundPose(collisionPose.Position, currentRotation, collisionPose.Heading, currentGround, deltaTime, out desiredPosition, out desiredRotation, out desiredGround) == false)
                {
                    ApplyRotationOnly(currentPosition, currentRotation, currentGround, desiredHeading, deltaTime);

                    return;
                }

                finalRequestedVelocity = slopeVelocity;
                finalSlideVelocity = slopeSlideVelocity;
            }
        }

        transform.SetPositionAndRotation(desiredPosition, desiredRotation);

        _headingRotation = collisionPose.Heading;

        Velocity = (desiredPosition - currentPosition) / deltaTime;

        Speed = collisionPose.Movement.magnitude / deltaTime;

        IsGrounded = true;

        if (IsSteep(desiredGround.Normal))
        {
            Vector3 resolvedVelocity = collisionPose.Movement / deltaTime;

            _slideVelocity = ResolveSlideVelocity(finalSlideVelocity, finalRequestedVelocity, resolvedVelocity, desiredGround.Normal);
        }
        else
        {
            _slideVelocity = Vector3.zero;
        }
    }

    private Quaternion GetDesiredHeading(Vector2 input, float deltaTime)
    {
        Vector3 inputDirection = new(input.x, 0f, input.y);

        if (inputDirection.sqrMagnitude < MinimumMagnitude)
        {
            return _headingRotation;
        }

        inputDirection.Normalize();

        Quaternion targetHeading = Quaternion.LookRotation(inputDirection, Vector3.up);

        return Quaternion.RotateTowards(_headingRotation, targetHeading, _rotationSpeed * deltaTime);
    }

    private Vector3 GetDriveVelocity(Vector2 input, Quaternion heading, Vector3 groundNormal)
    {
        if (input.sqrMagnitude < MinimumMagnitude)
        {
            return Vector3.zero;
        }

        Vector3 direction = Vector3.ProjectOnPlane(heading * Vector3.forward, groundNormal);

        if (direction.sqrMagnitude < MinimumMagnitude)
        {
            return Vector3.zero;
        }

        direction.Normalize();

        Vector3 velocity = direction * _speed;

        if (IsSteep(groundNormal) == false)
            return velocity;

        Vector3 downhill = Vector3.ProjectOnPlane(Vector3.down, groundNormal);

        if (downhill.sqrMagnitude < MinimumMagnitude)
        {
            return velocity;
        }

        downhill.Normalize();

        Vector3 uphill = -downhill;

        float uphillSpeed = Vector3.Dot(velocity, uphill);

        if (uphillSpeed <= 0f)
            return velocity;

        velocity -= uphill * uphillSpeed * _slopeSpeedMultiplier;

        return velocity;
    }

    private Vector3 GetSlideVelocity(Vector3 slideVelocity, Vector3 groundNormal, float deltaTime)
    {
        if (IsSteep(groundNormal) == false)
            return Vector3.zero;

        slideVelocity = Vector3.ProjectOnPlane(slideVelocity, groundNormal);

        Vector3 slopeGravity = Vector3.ProjectOnPlane(Vector3.down * _slideGravity, groundNormal);

        slideVelocity += slopeGravity * deltaTime;

        return Vector3.ClampMagnitude(slideVelocity, _maximumSlideSpeed);
    }

    private Vector3 ResolveSlideVelocity(Vector3 slideVelocity, Vector3 requestedVelocity, Vector3 resolvedVelocity, Vector3 groundNormal)
    {
        slideVelocity = Vector3.ProjectOnPlane(slideVelocity, groundNormal);

        if (slideVelocity.sqrMagnitude < MinimumMagnitude)
            return Vector3.zero;

        Vector3 slideDirection = slideVelocity.normalized;

        Vector3 blockedVelocity = requestedVelocity - resolvedVelocity;

        float blockedSlideSpeed = Vector3.Dot(blockedVelocity, slideDirection);

        if (blockedSlideSpeed > 0f)
        {
            float correction = Mathf.Min(blockedSlideSpeed, slideVelocity.magnitude);

            slideVelocity -= slideDirection * correction;
        }

        return Vector3.ClampMagnitude(slideVelocity, _maximumSlideSpeed);
    }

    private bool IsSteep(Vector3 normal)
    {
        return Vector3.Angle(normal, Vector3.up) > _maximumClimbAngle;
    }

    private bool TryGetGroundPose(Vector3 position, Quaternion currentRotation, Quaternion heading, GroundInfo referenceGround, float deltaTime, out Vector3 groundedPosition, out Quaternion groundedRotation, out GroundInfo ground)
    {
        Quaternion predictedTargetRotation = GetGroundRotation(heading, referenceGround.Normal);

        Quaternion predictedRotation = Quaternion.RotateTowards(currentRotation, predictedTargetRotation, _rotationSpeed * deltaTime);

        ground = _groundDetector.Detect(position, predictedRotation, Vector3.down);

        if (ground.IsGrounded == false)
        {
            groundedPosition = default;
            groundedRotation = default;

            return false;
        }

        Quaternion targetRotation = GetGroundRotation(heading, ground.Normal);

        groundedRotation = Quaternion.RotateTowards(currentRotation, targetRotation, _rotationSpeed * deltaTime);

        ground = _groundDetector.Detect(position, groundedRotation, Vector3.down);

        if (ground.IsGrounded == false)
        {
            groundedPosition = default;
            groundedRotation = default;

            return false;
        }

        groundedPosition = position + Vector3.down * ground.Offset;

        return true;
    }

    private void ApplyRotationOnly(Vector3 currentPosition, Quaternion currentRotation, GroundInfo currentGround, Quaternion desiredHeading, float deltaTime)
    {
        CollisionPose collisionPose = _collisionSolver.ResolvePose(currentPosition, _headingRotation, desiredHeading, Vector3.zero);

        if (TryGetGroundPose(collisionPose.Position, currentRotation, collisionPose.Heading, currentGround, deltaTime, out Vector3 position, out Quaternion rotation, out _) == false)
        {
            ApplyGroundPose(currentPosition, currentRotation, _headingRotation, currentGround, deltaTime);

            return;
        }

        transform.SetPositionAndRotation(position, rotation);

        _headingRotation = collisionPose.Heading;

        Velocity = (position - currentPosition) / deltaTime;
    }

    private void ApplyGroundPose(Vector3 position, Quaternion rotation, Quaternion planarRotation, GroundInfo ground, float deltaTime)
    {
        Quaternion targetRotation = GetGroundRotation(planarRotation, ground.Normal);
        Quaternion desiredRotation = Quaternion.RotateTowards(rotation, targetRotation, _rotationSpeed * deltaTime);

        GroundInfo desiredGround = _groundDetector.Detect(position, desiredRotation, Vector3.down);

        if (desiredGround.IsGrounded == false)
            return;

        Vector3 desiredPosition = position + Vector3.down * desiredGround.Offset;

        transform.SetPositionAndRotation(desiredPosition, desiredRotation);
    }

    private Quaternion GetGroundRotation(Quaternion planarRotation, Vector3 groundNormal)
    {
        Vector3 forward = Vector3.ProjectOnPlane(planarRotation * Vector3.forward, groundNormal);

        if (forward.sqrMagnitude < MinimumMagnitude)
            return planarRotation;

        return Quaternion.LookRotation(forward.normalized, groundNormal);
    }

    private Quaternion GetPlanarRotation(Quaternion rotation)
    {
        Vector3 forward = Vector3.ProjectOnPlane(rotation * Vector3.forward, Vector3.up);

        if (forward.sqrMagnitude < MinimumMagnitude)
            return Quaternion.identity;

        return Quaternion.LookRotation(forward.normalized, Vector3.up);
    }
}