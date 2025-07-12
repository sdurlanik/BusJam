using Sdurlanik.BusJam.UI;
using Sdurlanik.BusJam.UI.States;
using Sdurlanik.BusJam.UI.Views;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Sdurlanik.BusJam.Installers
{
    public class UIInstaller : MonoInstaller
    {
        [SerializeField] private  GameObject _startScreenPanel;
        [SerializeField] private  GameObject _gameplayPanel;
        [SerializeField] private  EndScreenView  _endScreenView;
        
        public override void InstallBindings()
        {
            Container.Bind<IUIState>().To<UIStartState>().AsSingle().WithArguments(_startScreenPanel);
            Container.Bind<IUIState>().To<UIGameplayState>().AsSingle().WithArguments(_gameplayPanel);
            Container.Bind<IUIState>().To<UIEndGameState>().AsSingle().WithArguments(_endScreenView);

            
            Container.BindInterfacesAndSelfTo<UIStateMachine>().AsSingle().NonLazy();
        }
    }
}