using Sdurlanik.BusJam.Core;
using Sdurlanik.BusJam.Core.BusSystem;
using Sdurlanik.BusJam.Core.Grid;
using Sdurlanik.BusJam.MVC.Controllers;
using Zenject;

namespace Sdurlanik.BusJam.Installers
{
    public class GameLogicInstaller :MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<CharacterMovementController>().AsSingle().NonLazy();
            Container.Bind<IGridSystemManager>().To<GridSystemManager>().AsSingle();
            Container.Bind<ILevelController>().To<LevelController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<BusSystemManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<WaitingAreaController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameStateManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<TimerController>().AsSingle();
        }
    }
}