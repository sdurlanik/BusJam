using Sdurlanik.BusJam.Controllers;
using Sdurlanik.BusJam.Core;
using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.Core.Factories;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject _characterPrefab;
        [SerializeField] private GameObject _obstaclePrefab;
        
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<LevelLoadRequestedSignal>();
            Container.DeclareSignal<LevelReadySignal>();
            Container.DeclareSignal<LevelSuccessSignal>();
            Container.DeclareSignal<LevelFailSignal>();
            
            Container.Bind<ICharacterFactory>().To<CharacterFactory>().AsSingle().WithArguments(_characterPrefab);
            Container.Bind<IObstacleFactory>().To<ObstacleFactory>().AsSingle().WithArguments(_obstaclePrefab);

            
            Container.Bind<IGridManager>().To<GridManager>().AsSingle();
            Container.Bind<ILevelController>().To<LevelController>().AsSingle().NonLazy();

        }
    }
}