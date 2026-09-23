using System.Collections;
using UnityEngine;

public class RoverAttractor : MonoBehaviour
{
    [SerializeField] private float _magneticRadius = 5f;
    [SerializeField] private float _findingFrequency = 1f;
    [SerializeField] private LayerMask _attractableMask;
    [SerializeField] private int _maxHits = 16;
    [SerializeField] private ResourceStorage _resourceContainer;
    [SerializeField] private int _maxResourceTier = 0;

    private Collider[] _hitBuffer;

    private Coroutine _findingCoroutine;

    private void Awake()
    {
        _hitBuffer = new Collider[_maxHits];
    }

    private void OnEnable()
    {
        _findingCoroutine = StartCoroutine(FindAttractable());
    }

    private void OnDisable()
    {
        if (_findingCoroutine != null)
        {
            StopCoroutine(_findingCoroutine);
            _findingCoroutine = null;
        }
    }

    private IEnumerator FindAttractable()
    {
        WaitForSeconds wait = new(_findingFrequency);

        while (enabled)
        {
            Attract();

            yield return wait;
        }
    }

    private void Attract()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _magneticRadius, _hitBuffer, _attractableMask, QueryTriggerInteraction.Collide);

        for (int i = 0; i < hitCount; i++)
        {
            if (_hitBuffer[i].TryGetComponent(out IAttractable attractable) == false)
                continue;

            if (attractable is IResource resource)
            {
                if (resource.Config.Tier > _maxResourceTier)
                    continue;

                if (_resourceContainer.CanAccept(resource.Config) == false)
                    continue;
            }

            attractable.SetTarget(transform);
        }
    }
}