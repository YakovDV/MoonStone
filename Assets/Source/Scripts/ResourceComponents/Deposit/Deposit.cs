using System;
using UnityEngine;

public class Deposit : MonoBehaviour, IMineable
{
    [SerializeField] private DepositConfig _config;
    [SerializeField] private ResourceSpawnerDispatcher _resourceSpawnDispatcher;

    public int Tier => _config.Tier;
    public int CurrentIntegrity { get; private set; }

    public DepositConfig Config => _config;

    public event Action<int> ValueChanged;
    public event Action<Deposit> Destroyed;

    private void Start()
    {
        Initialize(_config, _resourceSpawnDispatcher);
    }

    public void Initialize(DepositConfig config, ResourceSpawnerDispatcher resourceSpawner)
    {
        _config = config;
        _resourceSpawnDispatcher = resourceSpawner;

        CurrentIntegrity = UnityEngine.Random.Range(config.MinIntegrity, config.MaxIntegrity + 1);
    }

    public bool TryMine(int damage)
    {
        if (damage <= 0)
            return false;

        if (CurrentIntegrity <= 0)
            return false;

        CurrentIntegrity -= damage;

        if (CurrentIntegrity <= 0)
        {
            CurrentIntegrity = 0;
            ValueChanged?.Invoke(CurrentIntegrity);

            SpawnFragments();

            Destroyed?.Invoke(this);
            return true;
        }

        ValueChanged?.Invoke(CurrentIntegrity);
        Debug.Log(CurrentIntegrity);

        return true;
    }

    public void ResetState()
    {
        gameObject.SetActive(false);
        CurrentIntegrity = 0;
        _config = null;
        _resourceSpawnDispatcher = null;
    }

    private void SpawnFragments()
    {
        foreach (ResourceDrop drop in _config.Drops)
        {
            if (UnityEngine.Random.value > drop.Chance)
                continue;

            int amount = UnityEngine.Random.Range(drop.MinAmount, drop.MaxAmount + 1);

            _resourceSpawnDispatcher.Spawn(drop.Resource, transform, amount);
        }
    }
}
