using UnityEngine;

public class StorageOccupancyView : MonoBehaviour
{
    [SerializeField] private ResourceStorage _storage;
    [SerializeField] private Transform _fill;
    [SerializeField] private float _minFillPositionY;
    [SerializeField] private float _maxFillPositionY;

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
        Vector3 fillPosition = _fill.transform.localPosition;

        float occupancy = Mathf.Clamp01(mass / _storage.MaxCapacity);
        float fillPositionY = Mathf.Lerp(_minFillPositionY, _maxFillPositionY, occupancy);

        _fill.transform.localPosition = new Vector3(fillPosition.x, fillPositionY, fillPosition.z);
    }
}