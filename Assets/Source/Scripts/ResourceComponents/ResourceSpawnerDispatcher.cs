using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class ResourceSpawnerDispatcher : MonoBehaviour
{
    [SerializeField] private ResourceSpawnerLink[] _resourceSpawnerLinks;
    [SerializeField] private float _explosionForce = 20f;
    [SerializeField] private float _explosionRadius = 10f;
    [SerializeField] private float _spawnZoneSizeModificator = 0.8f;

    private Dictionary<ResourceConfig, UniversalSpawner<Resource>> _spawners;

    private void Awake()
    {
        _spawners = _resourceSpawnerLinks.ToDictionary(x => x.Config, x => x.Spawner);
    }

    public void Spawn(ResourceConfig config, Transform spawnZone, int count)
    {
        var spawner = _spawners[config];

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPoint = CalculateSpawnPoint(spawnZone);

            Resource resource = spawner.Spawn(spawnPoint);

            resource.ResourceConsumed += ReturnResource;
            resource.Initialize(config);

            if (resource.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.AddExplosionForce(_explosionForce, spawnZone.position, _explosionRadius);
            }
        }
    }

    private Vector3 CalculateSpawnPoint(Transform spawnZone)
    {
        Vector3 point = new(Random.Range(-_spawnZoneSizeModificator, _spawnZoneSizeModificator), 0f, Random.Range(-_spawnZoneSizeModificator, _spawnZoneSizeModificator));

        return spawnZone.TransformPoint(point);
    }

    private void ReturnResource(Resource resource)
    {
        resource.ResourceConsumed -= ReturnResource;

        ResourceConfig config = resource.Config;

        resource.ResetState();

        _spawners[config].Despawn(resource);
    }
}