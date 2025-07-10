using System;
using System.Collections.Generic;
using System.Linq;
using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.UI.States;
using Zenject;

namespace Sdurlanik.BusJam.UI
{
    public class UIStateMachine : IUIStateMachine, IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly List<IUIState> _states;
        private IUIState _currentState;

        public UIStateMachine(SignalBus signalBus, List<IUIState> states)
        {
            _signalBus = signalBus;
            _states = states;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<LevelReadySignal>(OnLevelReady);
            _signalBus.Subscribe<GameOverSignal>(OnGameOver);
            _signalBus.Subscribe<LevelCompleteSequenceFinishedSignal>(OnLevelCompleteSequenceFinished);
            ChangeState<UIStartState>();
        }
        
        public void Dispose()
        {
            _signalBus.TryUnsubscribe<LevelReadySignal>(OnLevelReady);
            _signalBus.TryUnsubscribe<GameOverSignal>(OnGameOver);
            _signalBus.TryUnsubscribe<LevelCompleteSequenceFinishedSignal>(OnLevelCompleteSequenceFinished);
        }
        

        public void ChangeState<T>(object payload = null) where T : IUIState
        {
            _currentState?.Exit();
            
            var newState = _states.FirstOrDefault(s => s is T);
            if (newState != null)
            {
                _currentState = newState;
                _currentState.Enter(payload);
            }
        }
        
        private void OnLevelReady() => ChangeState<UIGameplayState>();
        private void OnGameOver() => ChangeState<UIEndGameState>(false);
        private void OnLevelCompleteSequenceFinished() => ChangeState<UIEndGameState>(true);
    }
}