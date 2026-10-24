using UnityEngine;

public class LevelPopulation : MonoBehaviour
{
    [SerializeField] private ResourceScatterZone[] _resourceScatterZones;

    [SerializeField] private ResourceSpawnerDispatcher _resourceSpawnerDispatcher;
    [SerializeField] private DepositSpawnDispatcher _depositSpawnDispatcher;

    public void Initialize()
    {
        foreach (ResourceScatterZone zone in _resourceScatterZones)
            zone.Initialize(_resourceSpawnerDispatcher);
    }

    public void SpawnInitial()
    {
        foreach(ResourceScatterZone zone in _resourceScatterZones)
            zone.SpawnResources();

        _depositSpawnDispatcher.SpawnInitial();
    }

    public void DespawnAll()
    {
        _depositSpawnDispatcher.DespawnAll();
        _resourceSpawnerDispatcher.ReturnAllResources();
    }
}