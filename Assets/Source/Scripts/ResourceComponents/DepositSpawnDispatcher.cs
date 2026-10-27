using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DepositSpawnDispatcher : MonoBehaviour
{
    [SerializeField] private DepositSpawnerLink[] _depositSpawnerLinks;
    [SerializeField] private ResourceSpawnerDispatcher _resourceSpawnerDispatcher;
    [SerializeField] private DepositSpawnZone[] _spawnZones;

    private Dictionary<DepositConfig, UniversalSpawner<Deposit>> _spawners;

    private void Awake()
    {
        _spawners = _depositSpawnerLinks.ToDictionary(x => x.Config, x => x.Spawner);
    }

    public void SpawnInitial()
    {
        for (int i = 0; i < _spawnZones.Length; i++)
        {
            Spawn(_spawnZones[i].DepositConfig, _spawnZones[i].Zone, _spawnZones[i].DepositCount);
        }
    }

    public void Spawn(DepositConfig config, BoxCollider spawnZone, int count)
    {
        var spawner = _spawners[config];

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPoint = CalculateSpawnPoint(spawnZone);

            Deposit deposit = spawner.Spawn(spawnPoint);
            deposit.Initialize(config, _resourceSpawnerDispatcher);

            deposit.Destroyed += ReturnDeposit;
        }
    }

    public void DespawnAll()
    {
        foreach (var link in _depositSpawnerLinks)
        {
            var spawner = link.Spawner;
            spawner.ForEachActive(deposit => ReturnDeposit(deposit));
        }
    }

    private Vector3 CalculateSpawnPoint(BoxCollider zone)
    {
        Vector3 point = new(Random.Range(-zone.size.x * 0.5f, zone.size.x * 0.5f), 0f, Random.Range(-zone.size.z * 0.5f, zone.size.z * 0.5f));

        return zone.transform.TransformPoint(point);
    }

    private void ReturnDeposit(Deposit deposit)
    {
        deposit.Destroyed -= ReturnDeposit;

        DepositConfig config = deposit.Config;

        deposit.ResetState();

        _spawners[config].Despawn(deposit);
    }
}