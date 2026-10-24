using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DepositSpawnDispatcher : MonoBehaviour
{
    [SerializeField] private DepositSpawnerLink[] _depositSpawnerLinks;
    [SerializeField] private ResourceSpawnerDispatcher _resourceSpawnerDispatcher;
    [SerializeField] private DepositSpawnZone[] _spawnZones;

    private Dictionary<DepositConfig, UniversalSpawner<Deposit>> _spawners;
    private HashSet<Deposit> _deposits = new();

    private void Awake()
    {
        _spawners = _depositSpawnerLinks.ToDictionary(x => x.Config, x => x.Spawner);
    }

    public void SpawnInitial()
    {
        foreach (var zone in _spawnZones)
        {
            Spawn(zone.DepositConfig, zone.Zone, zone.DepositCount);
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

            _deposits.Add(deposit);

            deposit.Destroyed += ReturnDeposit;
        }
    }

    public void DespawnAll()
    {
        foreach (var deposit in _deposits.ToArray())
            ReturnDeposit(deposit);
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

        _deposits.Remove(deposit);

        _spawners[config].Despawn(deposit);
    }
}