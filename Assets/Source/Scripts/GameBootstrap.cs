using UnityEngine;
using YG;
using Zenject;

public class GameBootstrap : MonoBehaviour
{
    [Inject] private SaveService _saveService;
    [Inject] private ISaveStorage _saveStorage;
    [Inject] private SettingsInitializer _settingsInitializer;
    [Inject] private SceneNavigator _sceneNavigator;

    private void Start()
    {
        YG2.onGetSDKData += InitializeSaveData;

        if (YG2.isSDKEnabled)
            InitializeSaveData();
    }

    private void InitializeSaveData()
    {
        YG2.onGetSDKData -= InitializeSaveData;

        _saveService.Initialize(_saveStorage);
        _settingsInitializer.Initialize();

        _sceneNavigator.LoadScene(SceneID.MainMenu);
    }
}
