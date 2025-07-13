using System;
using System.Linq;
using Sdurlanik.BusJam.Core.BusSystem;
using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.Core.Grid;
using Sdurlanik.BusJam.Core.Movement;
using Sdurlanik.BusJam.Core.State;
using Sdurlanik.BusJam.MVC.Controllers;
using Sdurlanik.BusJam.MVC.Views;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.Core
{
    public class GameStateManager : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly IGridSystemManager _gridSystemManager;
        private readonly IBusSystemManager _busSystemManager;
        private readonly IWaitingAreaController _waitingAreaController;
        private readonly IMovementTracker _movementTracker;
        private readonly IGameplayStateHolder _gameplayStateHolder;
        private readonly ITimerController _timerController;
        
        private bool _isLevelWon;
        
        public GameStateManager(SignalBus signalBus, IGridSystemManager gridSystemManager, IWaitingAreaController waitingAreaController, IMovementTracker movementTracker, IBusSystemManager busSystemManager, IGameplayStateHolder gameplayStateHolder, ITimerController timerController)
        {
            _signalBus = signalBus;
            _gridSystemManager = gridSystemManager;
            _waitingAreaController = waitingAreaController;
            _movementTracker = movementTracker;
            _busSystemManager = busSystemManager;
            _gameplayStateHolder = gameplayStateHolder;
            _timerController = timerController;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<AllBusesDispatchedSignal>(OnAllBusesDispatched);
            _signalBus.Subscribe<RestartLevelRequestedSignal>(OnNewLevelSequenceRequested);
            _signalBus.Subscribe<NextLevelRequestedSignal>(OnNewLevelSequenceRequested);
            _signalBus.Subscribe<BusArrivedSignal>(OnBusArrived);
            _signalBus.Subscribe<TimeIsUpSignal>(OnTimeIsUp);
            _signalBus.Subscribe<CharacterEnteredWaitingAreaSignal>(OnCharacterEnteredWaitingArea);
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<AllBusesDispatchedSignal>(OnAllBusesDispatched);
            _signalBus.TryUnsubscribe<RestartLevelRequestedSignal>(OnNewLevelSequenceRequested);
            _signalBus.TryUnsubscribe<NextLevelRequestedSignal>(OnNewLevelSequenceRequested);
            _signalBus.TryUnsubscribe<BusArrivedSignal>(OnBusArrived);
            _signalBus.TryUnsubscribe<TimeIsUpSignal>(OnTimeIsUp);
            _signalBus.TryUnsubscribe<CharacterEnteredWaitingAreaSignal>(OnCharacterEnteredWaitingArea);
        }
        
        private void OnTimeIsUp() => ProcessGameOverState("Time is up!");
        private void OnCharacterEnteredWaitingArea() => CheckForWaitingAreaDeadlock();
        private void OnBusArrived() => CheckForWaitingAreaDeadlock();

        private void OnAllBusesDispatched()
        {
            if (_isLevelWon) return;

            CheckWinCondition();
        }

        private void CheckWinCondition()
        {
            var allGridObjects = _gridSystemManager.MainGrid.GetAllOccupiedObjects();

            var characterCountOnGrid = allGridObjects.Count(obj => obj.GetComponent<CharacterView>() != null);
    
            var waitingAreaCount = _waitingAreaController.GetWaitingCharacterCount();
    
            if (characterCountOnGrid == 0 && waitingAreaCount == 0)
            {
                ProcessWinState();
            }
            else
            {
                CheckForStuckCharacters();
            }
        }
        
        private void CheckForStuckCharacters()
        {
            if (!_gameplayStateHolder.IsGameplayActive) return;

            var allGridObjects = _gridSystemManager.MainGrid.GetAllOccupiedObjects();
            var characterCountOnGrid = allGridObjects.Count(obj => obj.GetComponent<CharacterView>() != null);
            var waitingAreaCount = _waitingAreaController.GetWaitingCharacterCount();

            if (characterCountOnGrid > 0 || waitingAreaCount > 0)
            {
                ProcessGameOverState("Game ended with stuck characters.");
            }
        }
        
        private void CheckForWaitingAreaDeadlock()
        {
            if (!_gameplayStateHolder.IsGameplayActive || _busSystemManager.IsBusInTransition || !_waitingAreaController.IsFull())
            {
                return;
            }

            var currentBus = _busSystemManager.CurrentBus;
            if (currentBus == null)
            {
                if (_waitingAreaController.GetWaitingCharacterCount() > 0)
                {
                    ProcessGameOverState("No more buses available, but characters are waiting.");
                }
                return;
            }

            var busColor = currentBus.GetColor();
            bool canAnyoneBoard = _waitingAreaController.GetWaitingCharacters().Any(c => c != null && c.Color == busColor);

            if (!canAnyoneBoard)
            {
                ProcessGameOverState($"Deadlock: No matching character for the {busColor} bus.");
            }
        }
        
        private void ProcessGameOverState(string reason)
        {
            if (!_gameplayStateHolder.IsGameplayActive) return;
            
            _gameplayStateHolder.Pause();
            _timerController.Stop();
            
            _signalBus.Fire<GameOverSignal>();
            Debug.Log(reason);
        }
        
        private void ProcessWinState()
        {
            if (!_gameplayStateHolder.IsGameplayActive) return;

            _gameplayStateHolder.Pause();
            _timerController.Stop();
            _isLevelWon = true;
            _signalBus.Fire<LevelCompletedSignal>();
        }
        
        private void OnNewLevelSequenceRequested()
        {
            _gameplayStateHolder.Resume();
            _movementTracker.Reset();
            _isLevelWon = false;
        }
    }
}