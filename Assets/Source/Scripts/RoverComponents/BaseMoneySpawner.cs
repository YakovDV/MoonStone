using System.Collections;
using UnityEngine;

public class BaseMoneySpawner : MonoBehaviour
{
    [SerializeField] private MoneySpawner _moneySpawner;
    [SerializeField] private Transform _moneySpawnPoint;
    [SerializeField] private float _spawnRandomness = 0.2f;
    [SerializeField] private float _frequency = 0.1f;
    [SerializeField] private Transform _explosionCenterPoint;
    [SerializeField] private float _minForce = 300f;
    [SerializeField] private float _maxForce = 500f;
    [SerializeField] private int _moneyWorth = 100;

    [SerializeField] private MoneyPickupEffectPlayer _effectPlayer;

    private int _remainingValue;
    private Coroutine _spawnCoroutine;

    public void Spawn(int value)
    {
        if (value <= 0)
            return;

        _remainingValue += value;

        if (_spawnCoroutine == null)
            _spawnCoroutine = StartCoroutine(SpawnFrequently());
    }

    private IEnumerator SpawnFrequently()
    {
        WaitForSeconds wait = new(_frequency);

        while (_remainingValue > 0)
        {
            int worth = Mathf.Min(_moneyWorth, _remainingValue);

            Money money = _moneySpawner.Spawn(_moneySpawnPoint.position);
            money.Initialize(worth);
            money.Consumed += OnMoneyConsumed;

            if (money.TryGetComponent(out Rigidbody rigidbody))
            {
                Vector3 direction = (_moneySpawnPoint.position - _explosionCenterPoint.position).normalized;
                Vector3 randomDirection = direction + Random.insideUnitSphere * _spawnRandomness;

                randomDirection.Normalize();

                float force = Random.Range(_minForce, _maxForce);

                rigidbody.AddForce(randomDirection * force, ForceMode.Impulse);
            }

            _remainingValue -= worth;

            yield return wait;
        }

        _spawnCoroutine = null;
    }

    private void OnMoneyConsumed(Money money)
    {
        _effectPlayer.PlayEffectAtPoint(money.transform.position);

        money.Consumed -= OnMoneyConsumed;
        money.ResetState();
        _moneySpawner.Despawn(money);
    }
}
