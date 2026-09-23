using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class StorageOccupancyBar : MonoBehaviour
{
    [SerializeField] private float _sliderSpeed = 1.0f;
    [SerializeField] private ResourceStorage _resourceStorage;

    private Slider _slider;
    private Coroutine _smoothSliderCoroutine;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (_resourceStorage == null)
        {
            return;
        }

        SetStorage(_resourceStorage);
    }

    private void OnDisable()
    {
        _resourceStorage.MassChanged -= ShowOccupancy;
    }

    public void SetStorage(ResourceStorage storage)
    {
        if (_resourceStorage != null)
        {
            _resourceStorage.MassChanged -= ShowOccupancy;
        }

        if (_slider.gameObject.activeSelf == false)
        {
            _slider.gameObject.SetActive(true);
        }

        _resourceStorage = storage;

        _slider.maxValue = _resourceStorage.MaxCapacity;
        _slider.minValue = 0f;

        _slider.value = 0f;

        _resourceStorage.MassChanged += ShowOccupancy;
        ShowOccupancy(_resourceStorage.CurrentResourcesMass);
    }

    private void ShowOccupancy(float mass)
    {
        if (_smoothSliderCoroutine != null)
        {
            StopCoroutine(_smoothSliderCoroutine);
            _smoothSliderCoroutine = null;
        }

        _smoothSliderCoroutine = StartCoroutine(ChangeSliderSmooth(mass));
    }

    private IEnumerator ChangeSliderSmooth(float health)
    {
        while (Mathf.Approximately(_slider.value, health) == false)
        {
            _slider.value = Mathf.MoveTowards(_slider.value, health, _sliderSpeed * Time.deltaTime);

            yield return null;
        }

        _slider.value = health;
    }
}
