using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private OrderConfig _orderConfig;
    [SerializeField] private UpgradeData _upgradeData;


    public override void InstallBindings()
    {
        Container.Bind<Rover>().FromComponentInHierarchy().AsSingle();

        Container.Bind<RoverBase>().FromComponentInHierarchy().AsSingle();

        Container.Bind<PanelSwitcher>().FromComponentInHierarchy().AsSingle();

        Container.Bind<BaseMoneySpawner>().FromComponentInHierarchy().AsSingle();

        Container.Bind<MoneyCollector>().FromComponentInHierarchy().AsSingle();

        Container.Bind<PlayerInputReader>().FromComponentInHierarchy().AsSingle();

        Container.Bind<LevelPopulation>().FromComponentInHierarchy().AsSingle();

        Container.Bind<TerrainTrackMask>().FromComponentInHierarchy().AsSingle();

        Container.BindInstance(_orderConfig);
        Container.BindInstance(_upgradeData);
    }
}
