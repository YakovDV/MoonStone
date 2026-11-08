using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioSettingsSaveData _audioSettingsSaveData;
    [SerializeField] private UpgradeData _initialUpgradeData;

    [SerializeField] private MusicPlayer _musicPlayer;

    public override void InstallBindings()
    {
        Container.Bind<SaveService>().AsSingle().WithArguments(new GameSaveData(0, 0, _initialUpgradeData), new SettingsSaveData(_audioSettingsSaveData));
        Container.Bind<AudioVolumeSettings>().AsSingle().WithArguments(_audioMixer);
        Container.BindInterfacesTo<SettingsInitializer>().AsSingle();

        Container.Bind<MusicPlayer>().FromComponentInNewPrefab(_musicPlayer).AsSingle().NonLazy();

        Container.Bind<SceneNavigator>().AsSingle();
        Container.Bind<Wallet>().AsSingle();
    }
}