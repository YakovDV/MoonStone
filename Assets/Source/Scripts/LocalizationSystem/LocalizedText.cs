using TMPro;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(TextMeshProUGUI))]
public sealed class LocalizedText : MonoBehaviour
{
    [SerializeField] private string _key;

    [Inject] private LocalizationService _localization;

    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if (_localization != null)
        {
            _localization.LanguageChanged += Refresh;

            Refresh();
        }
    }

    private void OnDisable()
    {
        if (_localization != null)
        {
            _localization.LanguageChanged -= Refresh;
        }
    }

    private void Refresh()
    {
        _text.text = _localization.Get(_key);
    }
}
