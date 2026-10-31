using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private UpgradeData _initialUpgradeData;

    public override void InstallBindings()
    {
        Container.Bind<Wallet>().AsSingle();
        Container.Bind<SaveService>().AsSingle().WithArguments(new SaveData(0, 0, _initialUpgradeData));
    }
}