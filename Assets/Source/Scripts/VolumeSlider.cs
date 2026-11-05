using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]

public class VolumeSlider : MonoBehaviour
{
    private const float DecibelMultiplier = 20f;

    [SerializeField] private AudioMixerGroup _audioMixerGroup;
    [SerializeField] private string _channelName;

    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
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
            _audioMixerGroup.audioMixer.SetFloat(_channelName, -80f);
            return;
        }

        _audioMixerGroup.audioMixer.SetFloat(_channelName, Mathf.Log10(value) * DecibelMultiplier);
    }
}