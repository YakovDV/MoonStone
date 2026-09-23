using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ProgressBar : MonoBehaviour
{
    [SerializeField] private float _sliderSpeed = 10f;

    private Order _order;
    private Slider _slider;
    private Coroutine _smoothSliderCoroutine;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void OnDisable()
    {
        _order.ProgressChanged -= OnProgressChanged;
    }

    public void SetOrder(Order order)
    {
        if (_order != null)
        {
            _order.ProgressChanged -= OnProgressChanged;
        }

        if (_slider.gameObject.activeSelf == false)
        {
            _slider.gameObject.SetActive(true);
        }

        _order = order;

        _slider.maxValue = _order.RequiredValue;
        _slider.minValue = 0f;

        _slider.value = 0f;

        _order.ProgressChanged += OnProgressChanged;
        OnProgressChanged(_order.Progress);
    }

    private void OnProgressChanged(float mass)
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
