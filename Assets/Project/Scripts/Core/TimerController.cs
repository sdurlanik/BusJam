using System;
using Sdurlanik.BusJam.Core.Events;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.Core
{
    public class TimerController : ITickable, IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private float _remainingTime;
        private bool _isTimerRunning;
        
        private float _updateTimer;
        private const float UPDATE_INTERVAL = 1f;

        public TimerController(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }
        
        public void Initialize()
        {
            _signalBus.Subscribe<LevelReadySignal>(OnLevelReady);
            _signalBus.Subscribe<GameOverSignal>(StopTimer);
            _signalBus.Subscribe<LevelSuccessSignal>(StopTimer);
            _signalBus.Subscribe<ResetGameplaySignal>(StopTimer);
        }
        
        public void Dispose()
        {
            _signalBus.TryUnsubscribe<LevelReadySignal>(OnLevelReady);
            _signalBus.TryUnsubscribe<GameOverSignal>(StopTimer);
            _signalBus.TryUnsubscribe<LevelSuccessSignal>(StopTimer);
            _signalBus.TryUnsubscribe<ResetGameplaySignal>(StopTimer);
        }
        
        private void OnLevelReady(LevelReadySignal signal)
        {
            _remainingTime = signal.LevelData.TimeLimitInSeconds;
            _isTimerRunning = true;
            _updateTimer = UPDATE_INTERVAL;
        }

        private void StopTimer()
        {
            _isTimerRunning = false;
            Debug.Log("Timer Stopped/Reset.");
        }

        public void Tick()
        {
            if (!_isTimerRunning) return;

            if (_remainingTime > 0)
            {
                _remainingTime -= Time.deltaTime;
                _updateTimer += Time.deltaTime;

                if (_updateTimer >= UPDATE_INTERVAL)
                {
                    _updateTimer -= UPDATE_INTERVAL;
                    _signalBus.Fire(new TimerUpdatedSignal(_remainingTime));
                }
            }
            else
            {
                _remainingTime = 0;
                _isTimerRunning = false;
                
                _signalBus.Fire(new TimerUpdatedSignal(_remainingTime));

                Debug.Log("Time is up! Firing GameOverSignal.");
                _signalBus.Fire<GameOverSignal>();
            }
        }
    }
}