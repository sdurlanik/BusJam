using Sdurlanik.BusJam.UI;
using Sdurlanik.BusJam.UI.States;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.Installers
{
    public class UIInstaller : MonoInstaller
    {
        [SerializeField] private  GameObject _startScreenPanel;
        [SerializeField] private  GameObject _gameplayPanel;
        [SerializeField] private  GameObject _endScreenPanel;
        
        public override void InstallBindings()
        {
            Container.Bind<IUIState>().To<UIStartState>().AsSingle().WithArguments(_startScreenPanel);
            Container.Bind<IUIState>().To<UIGameplayState>().AsSingle().WithArguments(_gameplayPanel);
            Container.Bind<IUIState>().To<UIEndGameState>().AsSingle().WithArguments(_endScreenPanel);
            
            Container.BindInterfacesAndSelfTo<UIStateMachine>().AsSingle().NonLazy();
        }
    }
}