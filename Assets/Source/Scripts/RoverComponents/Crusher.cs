using System;
using UnityEngine;

public class Crusher : MonoBehaviour
{
    [SerializeField] private int _damage = 10;
    [SerializeField] private float _damageFrequency = 1f;
    [SerializeField] private BoxCollider _damageZone;
    [SerializeField] private LayerMask _damageMask;

    public int Level { get; private set; } = 1;

    public event Action<int> DamageChanged;
    public event Action<int> LevelChanged;

    private readonly Collider[] _hits = new Collider[10];
    private float _damageTimer;

    private void FixedUpdate()
    {
        _damageTimer += Time.fixedDeltaTime;

        if (_damageTimer < _damageFrequency)
            return;

        _damageTimer -= _damageFrequency;

        DoDamage();
    }

    private void DoDamage()
    {
        Vector3 center = _damageZone.transform.TransformPoint(_damageZone.center);
        Vector3 scale = _damageZone.transform.lossyScale;
        Vector3 halfExtents = Vector3.Scale(_damageZone.size * 0.5f, new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z)));

        int count = Physics.OverlapBoxNonAlloc(center, halfExtents, _hits, _damageZone.transform.rotation, _damageMask, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < count; i++)
        {
            if (_hits[i].TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(_damage);
        }
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