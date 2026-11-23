using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LevelCard : MonoBehaviour
{
    private const string TextSeparator = "/";

    [SerializeField] private Button _button;
    [SerializeField] private SceneID _sceneID;
    [SerializeField] private OrderConfig _orderConfig;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _requiredProgressText;
    [SerializeField] private Image _accsessible;
    [SerializeField] private Image _unaccsessible;
    [SerializeField] private Image _lockedTone;

    [Inject] private LocalizationService _localization;

    public event Action<SceneID> StartLevelRequested;

    private void Awake()
    {
        _title.text = _orderConfig.Name;
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(RequestStartLevel);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(RequestStartLevel);
    }

    public void RefreshInfo(bool available, int progress)
    {
        _button.interactable = available;

        _accsessible.enabled = available;
        _unaccsessible.enabled = !available;
        _lockedTone.enabled = !available;

        _requiredProgressText.enabled = !available;
        _requiredProgressText.text = progress.ToString() + TextSeparator + _orderConfig.RequiredProgress.ToString();

        RefreshName();
    }

    public int GetLevelRequiredProgress()
    {
        return _orderConfig.RequiredProgress;
    }

    private void RequestStartLevel()
    {
        StartLevelRequested?.Invoke(_sceneID);
    }

    private void RefreshName()
    {
        if (_orderConfig == null)
            return;

        _title.text = _localization.Get(_orderConfig.Name);
    }
}