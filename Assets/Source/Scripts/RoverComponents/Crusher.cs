using System;
using UnityEngine;

public class Crusher : MonoBehaviour
{
    [SerializeField] private int _tier = 4;
    [SerializeField] private int _damage = 10;
    [SerializeField] private float _damageFrequency = 1f;
    [SerializeField] private BoxCollider _damageZone;
    [SerializeField] private LayerMask _damageMask;
    [SerializeField] private CrusherEffector _effector;

    public int Level { get; private set; } = 1;

    public event Action<int> DamageChanged;
    public event Action<int> LevelChanged;

    private IMineable _currentTarget;
    private bool _isCrushing;
    private readonly Collider[] _hits = new Collider[10];
    private float _damageTimer;

    private void Awake()
    {
        Level = _tier;
    }

    private void FixedUpdate()
    {
        IMineable target = FindTarget();

        if (target == null)
        {
            StopCrushing();
            return;
        }

        int targetTier = target.Tier;

        if (ReferenceEquals(target, _currentTarget) == false)
        {
            StopCrushing();

            _currentTarget = target;
            _damageTimer = _damageFrequency;
        }

        _damageTimer += Time.fixedDeltaTime;

        if (_damageTimer < _damageFrequency)
            return;

        _damageTimer -= _damageFrequency;

        if (_currentTarget.TryMine(_damage) == false)
        {
            StopCrushing();
            return;
        }

        if (_isCrushing)
            return;

        _isCrushing = true;
        _effector.Play(targetTier);
    }

    private IMineable FindTarget()
    {
        Vector3 center = _damageZone.transform.TransformPoint(_damageZone.center);
        Vector3 scale = _damageZone.transform.lossyScale;
        Vector3 halfExtents = Vector3.Scale(_damageZone.size * 0.5f, new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z)));

        int count = Physics.OverlapBoxNonAlloc(center, halfExtents, _hits, _damageZone.transform.rotation, _damageMask, QueryTriggerInteraction.Ignore);

        IMineable closest = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            if (_hits[i].TryGetComponent(out IMineable mineable) == false)
                continue;

            if (mineable.Tier > Level)
                continue;

            if (ReferenceEquals(mineable, _currentTarget))
                return mineable;

            Vector3 point = _hits[i].ClosestPoint(center);
            float distance = (point - center).sqrMagnitude;

            if (distance >= closestDistance)
                continue;

            closest = mineable;
            closestDistance = distance;
        }

        return closest;
    }

    private void StopCrushing()
    {
        _currentTarget = null;
        _damageTimer = 0f;

        if (_isCrushing == false)
            return;

        _isCrushing = false;
        _effector.Stop();
    }

    public void Upgrade(int damageValue, int levelValue)
    {
        if (damageValue <= 0 || levelValue <= 0)
            return;

        _damage += damageValue;
        Level += levelValue;

        DamageChanged?.Invoke(_damage);
        LevelChanged?.Invoke(Level);
    }
}
