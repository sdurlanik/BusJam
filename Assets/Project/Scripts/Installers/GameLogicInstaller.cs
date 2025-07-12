using Sdurlanik.BusJam.Core;
using Sdurlanik.BusJam.Core.BusSystem;
using Sdurlanik.BusJam.Core.Grid;
using Sdurlanik.BusJam.Core.Movement;
using Sdurlanik.BusJam.Core.Pathfinding;
using Sdurlanik.BusJam.Core.State;
using Sdurlanik.BusJam.MVC.Controllers;
using Zenject;

namespace Sdurlanik.BusJam.Installers
{
    public class GameLogicInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ILevelController>().To<LevelController>().AsSingle().NonLazy();
            Container.Bind<IPathfindingService>().To<PathfindingService>().AsSingle();
            Container.BindInterfacesAndSelfTo<GridSystemManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<BusSystemManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<WaitingAreaController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameStateManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LevelProgressionManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<CharacterMovementController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<TimerController>().AsSingle();
            Container.BindInterfacesAndSelfTo<MovementTracker>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameplayStateHolder>().AsSingle();
        }
    }
}