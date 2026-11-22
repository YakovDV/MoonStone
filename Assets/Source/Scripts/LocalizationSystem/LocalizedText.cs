using TMPro;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(TextMeshProUGUI))]
public sealed class LocalizedText : MonoBehaviour
{
    [SerializeField] private string _key;

    private LocalizationService _localization;
    private TextMeshProUGUI _text;
    private bool _subscribed;

    [Inject]
    private void Construct(LocalizationService localization)
    {
        _localization = localization;
        _text = GetComponent<TextMeshProUGUI>();

        if (isActiveAndEnabled)
            Activate();
    }

    private void OnEnable()
    {
        if (_localization != null)
            Activate();
    }

    private void OnDisable()
    {
        if (_subscribed == false)
            return;

        _localization.LanguageChanged -= Refresh;
        _subscribed = false;
    }

    private void Activate()
    {
        if (_subscribed == false)
        {
            _localization.LanguageChanged += Refresh;
            _subscribed = true;
        }

        Refresh();
    }

    private void Refresh()
    {
        _text.text = _localization.Get(_key);
    }
}
