using UnityEngine;

public class ResourceScatterZone : MonoBehaviour
{
    [SerializeField] private ResourceDrop[] _resourceDrops;

    private ResourceSpawnerDispatcher _resourceSpawnerDispatcher;

    public void Initialize(ResourceSpawnerDispatcher spawnerDispatcher)
    {
        _resourceSpawnerDispatcher = spawnerDispatcher;
    }

    public void SpawnResources()
    {
        foreach (ResourceDrop drop in _resourceDrops)
        {
            if (Random.value > drop.Chance)
                continue;

            int amount = Random.Range(drop.MinAmount, drop.MaxAmount + 1);

            _resourceSpawnerDispatcher.Spawn(drop.Resource, transform, amount);
        }
    }
}
