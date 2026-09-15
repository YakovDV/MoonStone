using System;
using UnityEngine;

public class Deposit : MonoBehaviour, IDamageable
{
    [SerializeField] private DepositConfig _config;
    [SerializeField] private ResourceSpawnerDispatcher _resourceSpawner;

    public int CurrentIntegrity { get; private set; }

    public DepositConfig Config => _config;

    public event Action<int> ValueChanged;
    public event Action<Deposit> Destroyed;

    public void Initialize(DepositConfig config, ResourceSpawnerDispatcher fragmentSpawner)
    {
        _config = config;
        _resourceSpawner = fragmentSpawner;

        CurrentIntegrity = UnityEngine.Random.Range(config.MinIntegrity, config.MaxIntegrity + 1);
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0)
            return;

        if (CurrentIntegrity <= 0)
            return;

        CurrentIntegrity -= damage;

        if (CurrentIntegrity <= 0)
        {
            CurrentIntegrity = 0;
            ValueChanged?.Invoke(CurrentIntegrity);

            SpawnFragments();

            Destroyed?.Invoke(this);
            return;
        }

        ValueChanged?.Invoke(CurrentIntegrity);
        Debug.Log(CurrentIntegrity);
    }

    public void ResetState()
    {
        gameObject.SetActive(false);
        CurrentIntegrity = 0;
        _config = null;
        _resourceSpawner = null;
    }

    private void SpawnFragments()
    {
        foreach (ResourceDrop drop in _config.Drops)
        {
            if (UnityEngine.Random.value > drop.Chance)
                continue;

            int amount = UnityEngine.Random.Range(drop.MinAmount, drop.MaxAmount + 1);

            _resourceSpawner.Spawn(drop.Resource, transform, amount);
        }
    }
}
