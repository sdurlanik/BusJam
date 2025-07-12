using System;
using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.Core.State;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.Core
{
    public class TimerController : ITickable,ITimerController, IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly IGameplayStateHolder _gameplayStateHolder;
        
        private float _remainingTime;
        private bool _isTimerRunning;
        
        private float _visualUpdateTimer;
        private const float VISUAL_UPDATE_INTERVAL = 1f;

        public TimerController(SignalBus signalBus, IGameplayStateHolder gameplayStateHolder)
        {
            _signalBus = signalBus;
            _gameplayStateHolder = gameplayStateHolder;
        }
        
        public void Initialize()
        {
            _signalBus.Subscribe<LevelReadySignal>(OnLevelReady);
            _signalBus.Subscribe<ResetGameplaySignal>(Stop);
        }
        
        public void Dispose()
        {
            _signalBus.TryUnsubscribe<LevelReadySignal>(OnLevelReady);
            _signalBus.TryUnsubscribe<ResetGameplaySignal>(Stop);
        }
        
        private void OnLevelReady(LevelReadySignal signal)
        {
            _remainingTime = signal.LevelData.TimeLimitInSeconds;
            _isTimerRunning = true;
            _visualUpdateTimer = 0f;
            _signalBus.Fire(new TimerUpdatedSignal(_remainingTime));
        }

        public void Stop()
        {
            _isTimerRunning = false;
        }

        public void Tick()
        {
            if (!_isTimerRunning || !_gameplayStateHolder.IsGameplayActive) return;

            _remainingTime -= Time.deltaTime;
            _visualUpdateTimer -= Time.deltaTime;

            if (_visualUpdateTimer <= 0f)
            {
                _visualUpdateTimer = VISUAL_UPDATE_INTERVAL;
                _signalBus.Fire(new TimerUpdatedSignal(_remainingTime));
            }
            
            if (_remainingTime <= 0)
            {
                _remainingTime = 0;
                Stop();
                _signalBus.Fire<TimeIsUpSignal>();
            }
        }
    }
}