using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.Core.Grid;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.Installers
{
    public class ProjectSettingsInstaller : MonoInstaller
    {
        [SerializeField] private GridConfiguration _gridConfiguration;
        
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<LevelLoadRequestedSignal>();
            Container.DeclareSignal<LevelReadySignal>();
            Container.DeclareSignal<TimerUpdatedSignal>();
            Container.DeclareSignal<LevelSuccessSignal>();
            Container.DeclareSignal<CharacterClickedSignal>();
            Container.DeclareSignal<BusArrivedSignal>();
            Container.DeclareSignal<BusFullSignal>();
            Container.DeclareSignal<GameOverSignal>();
            Container.DeclareSignal<AllBusesDispatchedSignal>();
            
            Container.Bind<GridConfiguration>().FromInstance(_gridConfiguration).AsSingle();
        }
    }
}