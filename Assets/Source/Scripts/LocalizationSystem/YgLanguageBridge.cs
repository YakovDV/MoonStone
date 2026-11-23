using UnityEngine;
using YG;
using Zenject;

public sealed class YgLanguageBridge : MonoBehaviour
{
    [Inject] private LocalizationService _localization;

    private void OnEnable()
    {
        YG2.onGetSDKData += ApplySdkLanguage;
        YG2.onSwitchLang += OnLanguageChanged;

        if (YG2.isSDKEnabled)
            ApplySdkLanguage();
    }

    private void OnDestroy()
    {
        if (_localization == null)
            return;

        YG2.onGetSDKData -= ApplySdkLanguage;
        YG2.onSwitchLang -= OnLanguageChanged;
    }

    private void ApplySdkLanguage()
    {
        _localization.SetLanguage(YG2.lang);
    }

    private void OnLanguageChanged(string language)
    {
        _localization.SetLanguage(language);
    }
}