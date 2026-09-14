using UnityEngine;

public class DepositSpawnZone : MonoBehaviour
{
    [SerializeField] private BoxCollider _zone;
    [SerializeField] private int _depositCount;
    [SerializeField] private DepositConfig _depositConfig;

    public BoxCollider Zone => _zone;
    public int DepositCount => _depositCount;
    public DepositConfig DepositConfig => _depositConfig;
}