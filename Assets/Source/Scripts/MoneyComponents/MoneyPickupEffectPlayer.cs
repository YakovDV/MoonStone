using UnityEngine;

public class MoneyPickupEffectPlayer : MonoBehaviour
{
    [SerializeField] private MoneyPickupEffectSpawner _spawner;

    public void PlayEffectAtPoint(Vector3 position)
    {
        MoneyPickupEffect effect = _spawner.Spawn(position);

        effect.ReadyToReturn += OnReadyToReturn;

        effect.Play();
    }

    private void OnReadyToReturn(MoneyPickupEffect effect)
    {
        effect.ReadyToReturn -= OnReadyToReturn;
        _spawner.Despawn(effect);
    }
}