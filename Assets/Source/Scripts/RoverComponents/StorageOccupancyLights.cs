using UnityEngine;

public class StorageOccupancyLights : MonoBehaviour
{
    [SerializeField] private LightBox[] _lightBoxes;
    [SerializeField] private ResourceStorage _storage;

    private int _count;

    private void Awake()
    {
        _count = _lightBoxes.Length;
    }

    private void OnEnable()
    {
        _storage.MassChanged += OnMassChanged;
        OnMassChanged(_storage.CurrentResourcesMass);
    }

    private void OnDisable()
    {
        _storage.MassChanged -= OnMassChanged;
    }

    private void OnMassChanged(float mass)
    {
        float occupancy = mass / _storage.MaxCapacity;
        int activeBoxes = Mathf.RoundToInt(occupancy * _count);

        for (int i = 0; i < _count; i++)
        {
            if (i < activeBoxes)
                _lightBoxes[i].TurnOn();
            else
                _lightBoxes[i].TurnOff();
        }
    }
}
