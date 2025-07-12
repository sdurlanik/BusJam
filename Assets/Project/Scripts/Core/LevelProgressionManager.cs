using System;
using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.Core.Movement;
using Sdurlanik.BusJam.Models;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.Core
{
    public class LevelProgressionManager : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly LevelProgressionSO _levelProgression;
        private readonly IMovementTracker _movementTracker;
        
        private int _currentLevelIndex;
        private const string SAVE_KEY = "CurrentLevelIndex";

        public LevelProgressionManager(SignalBus signalBus, LevelProgressionSO levelProgression, IMovementTracker movementTracker)
        {
            _signalBus = signalBus;
            _levelProgression = levelProgression;
            _movementTracker = movementTracker;
        }

        public void Initialize()
        {
            _currentLevelIndex = PlayerPrefs.GetInt(SAVE_KEY, 0);
            
            _signalBus.Subscribe<StartGameRequestedSignal>(OnStartGameRequested);
            _signalBus.Subscribe<NextLevelRequestedSignal>(OnNextLevelRequested);
            _signalBus.Subscribe<RestartLevelRequestedSignal>(OnRestartLevelRequested);
            _signalBus.Subscribe<LevelCompletedSignal>(SaveProgress);
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<StartGameRequestedSignal>(OnStartGameRequested);
            _signalBus.TryUnsubscribe<NextLevelRequestedSignal>(OnNextLevelRequested);
            _signalBus.TryUnsubscribe<RestartLevelRequestedSignal>(OnRestartLevelRequested);
            _signalBus.TryUnsubscribe<LevelCompletedSignal>(SaveProgress);
        }
        
        private  void OnStartGameRequested()
        {
            LoadLevelByIndex(_currentLevelIndex);
        }
        
        private void OnRestartLevelRequested()
        {
            LoadLevelByIndex(_currentLevelIndex);
        }

        private async  void OnNextLevelRequested()
        {
            await _movementTracker.WaitForAllMovementsToComplete();
            LoadLevelByIndex(_currentLevelIndex);
        }

        private void LoadLevelByIndex(int index)
        {
            _signalBus.Fire<ResetGameplaySignal>();

            if (index < _levelProgression.Levels.Count)
            {
                var levelToLoad = _levelProgression.Levels[index];
                _signalBus.Fire(new LevelLoadRequestedSignal(levelToLoad));
            }
            else
            {
                Debug.LogError($"Level index {index} is out of bounds!");
            }
        }
        
        private void SaveProgress()
        {
            int nextLevelIndex = _currentLevelIndex + 1;
            if (nextLevelIndex >= _levelProgression.Levels.Count)
            {
                nextLevelIndex = 0;
            }
        
            _currentLevelIndex = nextLevelIndex;
            
            PlayerPrefs.SetInt(SAVE_KEY, nextLevelIndex);
            PlayerPrefs.Save();
            Debug.Log($"Progress Saved. Next level index is {nextLevelIndex}");
        }
    }
}