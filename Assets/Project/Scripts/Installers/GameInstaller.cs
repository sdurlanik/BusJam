using Sdurlanik.BusJam.Controllers;
using Sdurlanik.BusJam.Core;
using Sdurlanik.BusJam.Events;
using Zenject;

namespace Sdurlanik.BusJam.Installers
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<LevelLoadRequestedSignal>();
            Container.DeclareSignal<LevelReadySignal>();
            Container.DeclareSignal<LevelSuccessSignal>();
            Container.DeclareSignal<LevelFailSignal>();
            
            Container.Bind<IGridManager>().To<GridManager>().AsSingle();
            Container.Bind<ILevelController>().To<LevelController>().AsSingle().NonLazy();

        }
    }
}