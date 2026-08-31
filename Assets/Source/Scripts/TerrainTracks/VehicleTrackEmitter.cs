using UnityEngine;

[DisallowMultipleComponent]
public sealed class VehicleTrackEmitter : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private TerrainTrackMask _trackMask;
    [SerializeField] private Transform[] _trackSources;
    [SerializeField] private LayerMask _groundMask;

    [Header("Ground detection")]
    [SerializeField, Min(0f)] private float _rayStartOffset = 0.25f;
    [SerializeField, Min(0.01f)] private float _rayDistance = 1.5f;

    [Header("Track")]
    [SerializeField, Min(0.01f)] private float _sampleDistance = 0.15f;
    [SerializeField, Min(0.01f)] private float _maximumSegmentLength = 1.5f;
    [SerializeField, Min(0.01f)] private float _trackWidth = 0.5f;
    [SerializeField, Min(0.001f)] private float _edgeSoftness = 0.08f;

    private Vector3[] _previousContactPoints;
    private bool[] _hadContact;

    private void Awake()
    {
        int sourcesCount = _trackSources == null ? 0 : _trackSources.Length;

        _previousContactPoints = new Vector3[sourcesCount];
        _hadContact = new bool[sourcesCount];
    }

    private void OnEnable()
    {
        ResetContacts();
    }

    private void OnDisable()
    {
        ResetContacts();
    }

    private void Update()
    {
        if (_trackMask == null || _trackSources == null)
            return;

        for (int i = 0; i < _trackSources.Length; i++)
            UpdateTrackSource(i);
    }

    private void UpdateTrackSource(int index)
    {
        Transform source = _trackSources[index];

        if (source == null)
            return;

        if (TryGetGroundContact(source, out Vector3 contactPoint) == false)
        {
            _hadContact[index] = false;
            return;
        }

        if (!_hadContact[index])
        {
            _previousContactPoints[index] = contactPoint;
            _hadContact[index] = true;
            return;
        }

        Vector3 offset = contactPoint - _previousContactPoints[index];
        offset.y = 0f;

        float distanceSquared = offset.sqrMagnitude;

        if (distanceSquared < _sampleDistance * _sampleDistance)
            return;

        if (distanceSquared <= _maximumSegmentLength * _maximumSegmentLength)
        {
            _trackMask.QueueSegment(
                _previousContactPoints[index],
                contactPoint,
                _trackWidth,
                _edgeSoftness
            );
        }

        _previousContactPoints[index] = contactPoint;
    }

    private bool TryGetGroundContact(Transform source, out Vector3 contactPoint)
    {
        Vector3 rayOrigin = source.position + Vector3.up * _rayStartOffset;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, _rayDistance, _groundMask, QueryTriggerInteraction.Ignore))
        {
            contactPoint = hit.point;
            return true;
        }

        contactPoint = default;
        return false;
    }

    private void ResetContacts()
    {
        if (_hadContact == null)
            return;

        for (int i = 0; i < _hadContact.Length; i++)
            _hadContact[i] = false;
    }
}