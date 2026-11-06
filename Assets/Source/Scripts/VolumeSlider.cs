using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]

public class VolumeSlider : MonoBehaviour
{
    private const float DecibelMultiplier = 20f;

    [SerializeField] private AudioMixerGroup _audioMixerGroup;
    [SerializeField] private string _parameterName;

    [SerializeField] private float _sliderDefaultValue;

    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void Start()
    {
        _slider.SetValueWithoutNotify(_sliderDefaultValue);
        ChangeVolume(_sliderDefaultValue);
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
        if (value <= 0)
        {
            _audioMixerGroup.audioMixer.SetFloat(_parameterName, -80f);
            return;
        }

        _audioMixerGroup.audioMixer.SetFloat(_parameterName, Mathf.Log10(value) * DecibelMultiplier);
    }
}