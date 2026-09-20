using TMPro;
using UnityEngine;

public class ResourceStorageView : MonoBehaviour
{
    [SerializeField] private ResourceStorage _storage;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private string _mainText = "Container capacity: ";
    [SerializeField] private char _valuesSeparator = '/';

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
        _text.text = $"{_mainText}{mass:0}{_valuesSeparator}{_storage.MaxCapacity:0}";
    }
}