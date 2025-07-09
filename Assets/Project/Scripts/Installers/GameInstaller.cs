using Sdurlanik.BusJam.Controllers;
using Sdurlanik.BusJam.Core.BusSystem;
using Sdurlanik.BusJam.Core.Grid;
using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.Core.Factories;
using Sdurlanik.BusJam.MVC.Controllers;
using UnityEngine;
using Zenject;
using Grid = Sdurlanik.BusJam.Core.Grid.Grid;

namespace Sdurlanik.BusJam.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject _characterPrefab;
        [SerializeField] private GameObject _obstaclePrefab;
        [SerializeField] private GameObject _mainGridTilePrefab; 
        [SerializeField] private GameObject _waitingAreaTilePrefab; 
        [SerializeField] private GameObject _busPrefab;

        
        [Header("Configurations")]
        [SerializeField] private GridConfiguration _gridConfiguration;
        
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<LevelLoadRequestedSignal>();
            Container.DeclareSignal<LevelReadySignal>();
            Container.DeclareSignal<LevelSuccessSignal>();
            Container.DeclareSignal<LevelFailSignal>();
            Container.DeclareSignal<CharacterClickedSignal>();
            Container.DeclareSignal<BusArrivedSignal>();
            Container.DeclareSignal<BusFullSignal>();
            
            Container.Bind<GridConfiguration>().FromInstance(_gridConfiguration).AsSingle();
            Container.Bind<IGridSystemManager>().To<GridSystemManager>().AsSingle();

            
            Container.Bind<ICharacterFactory>().To<CharacterFactory>().AsSingle().WithArguments(_characterPrefab);
            Container.Bind<IObstacleFactory>().To<ObstacleFactory>().AsSingle().WithArguments(_obstaclePrefab);

            Container.Bind<IBusFactory>().To<BusFactory>().AsSingle().WithArguments(_busPrefab);
            Container.BindInterfacesAndSelfTo<BusSystemManager>().AsSingle().NonLazy();

            
            Container.Bind<IGrid>().To<Grid>().AsSingle();
            Container.Bind<ILevelController>().To<LevelController>().AsSingle().NonLazy();
            Container.Bind<IWaitingAreaController>().To<WaitingAreaController>().AsSingle().NonLazy();
            Container.Bind<CharacterMovementController>().AsSingle().NonLazy();

        }
    }
}