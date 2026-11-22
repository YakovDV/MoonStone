using UnityEngine;
using YG;
using Zenject;

public sealed class YgLanguageBridge : MonoBehaviour
{
    private LocalizationService _localization;

    private void OnDestroy()
    {
        if (_localization == null)
            return;

        YG2.onGetSDKData -= ApplySdkLanguage;
        YG2.onSwitchLang -= OnLanguageChanged;
    }

    [Inject]
    private void Construct(LocalizationService localization)
    {
        _localization = localization;

        YG2.onGetSDKData += ApplySdkLanguage;
        YG2.onSwitchLang += OnLanguageChanged;

        if (YG2.isSDKEnabled)
            ApplySdkLanguage();
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