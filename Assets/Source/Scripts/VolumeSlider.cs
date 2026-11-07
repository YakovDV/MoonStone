using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Slider))]

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private string _parameterName;

    [Inject] AudioVolumeSettings _audioVolumeSettings;

    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void Start()
    {
        float value = _audioVolumeSettings.GetVolume(_parameterName);
        _slider.SetValueWithoutNotify(value);
    }

    private void OnEnable()
    {
        _slider.onValueChanged.AddListener(ChangeVolume);
    }

    private void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(ChangeVolume);
    }

    private void ChangeVolume(float value)
    {
        _audioVolumeSettings.SetVolume(_parameterName, value);
    }
}
