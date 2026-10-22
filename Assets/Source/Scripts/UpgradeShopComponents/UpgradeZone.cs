using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UpgradeZone : MonoBehaviour
{
    [SerializeField] private UpgradeShop _upgradeShop;

    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Rover rover = other.GetComponentInParent<Rover>();

        if (rover == null)
        {
            return; 
        }

        _upgradeShop.Open(rover);
    }

    private void OnTriggerExit(Collider other)
    {
        Rover rover = other.GetComponentInParent<Rover>();

        if (rover == null)
        {
            return;
        }

        _upgradeShop.Close();
    }
}
