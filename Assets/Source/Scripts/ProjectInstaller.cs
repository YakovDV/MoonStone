using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioSettingsSaveData _audioSettingsSaveData;
    [SerializeField] private UpgradeData _initialUpgradeData;

    [SerializeField] private MusicPlayer _musicPlayer;
    [SerializeField] private SFXPlayer _sfxPlayer;

    [SerializeField] private SceneCatalog _sceneCatalog;
    [SerializeField] private ScreenFader _sceneFader;

    public override void InstallBindings()
    {
        Container.Bind<SaveService>().AsSingle().WithArguments(new GameSaveData(0, 0, _initialUpgradeData), new SettingsSaveData(_audioSettingsSaveData));
        Container.Bind<AudioVolumeSettings>().AsSingle().WithArguments(_audioMixer);
        Container.BindInterfacesTo<SettingsInitializer>().AsSingle();

        Container.Bind<MusicPlayer>().FromComponentInNewPrefab(_musicPlayer).AsSingle().NonLazy();
        Container.Bind<SFXPlayer>().FromComponentInNewPrefab(_sfxPlayer).AsSingle().NonLazy();

        Container.BindInstance(_sceneCatalog);
        Container.BindInstance(_sceneFader);
        Container.Bind<SceneNavigator>().AsSingle();

        Container.Bind<Wallet>().AsSingle();
    }
}