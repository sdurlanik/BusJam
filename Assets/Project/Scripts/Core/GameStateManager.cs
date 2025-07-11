using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.Core.Grid;
using Sdurlanik.BusJam.MVC.Controllers;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using DG.Tweening;
using Sdurlanik.BusJam.Core.BusSystem;
using Sdurlanik.BusJam.Core.Movement;
using Sdurlanik.BusJam.Core.State;

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
        
        private bool _isLevelWon = false;
        private bool _isLevelLost = false;

        private bool IsGameResolved => _isLevelWon || _isLevelLost;
        
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
            _isLevelWon = false;
            _gameplayStateHolder.Resume();
            
            _signalBus.Subscribe<BusFullSignal>(OnBusFull);
            _signalBus.Subscribe<AllBusesDispatchedSignal>(OnAllBusesDispatched);
            _signalBus.Subscribe<RestartLevelRequestedSignal>(OnNewLevelSequenceRequested);
            _signalBus.Subscribe<NextLevelRequestedSignal>(OnNewLevelSequenceRequested);
            _signalBus.Subscribe<BusArrivedSignal>(OnBusArrived);
            _signalBus.Subscribe<TimeIsUpSignal>(OnTimeIsUp);
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<BusFullSignal>(OnBusFull);
            _signalBus.TryUnsubscribe<AllBusesDispatchedSignal>(OnAllBusesDispatched);
            _signalBus.TryUnsubscribe<RestartLevelRequestedSignal>(OnNewLevelSequenceRequested);
            _signalBus.TryUnsubscribe<NextLevelRequestedSignal>(OnNewLevelSequenceRequested);
            _signalBus.TryUnsubscribe<BusArrivedSignal>(OnBusArrived);
            _signalBus.TryUnsubscribe<TimeIsUpSignal>(OnTimeIsUp);
        }
        
        private void OnTimeIsUp()
        {
            ProcessGameOverState();
        }

        private void OnBusFull()
        {
            CheckWinCondition();  
        } 

        private void OnBusArrived() => CheckForWaitingAreaDeadlock();
        
        private void OnAllBusesDispatched()
        {
            Debug.Log( "All buses have been dispatched. Checking game state...");
            if (_isLevelWon)
            {
                Debug.Log("Level already won, skipping checks.");
                _signalBus.Fire<LevelCompleteSequenceFinishedSignal>();
                return;
            }
    
            CheckForStuckCharacters(); 
        } 
        
        private void CheckWinCondition()
        {
            Debug.Log( "Checking win condition...");
            if (!_gameplayStateHolder.IsGameplayActive) return;

            var mainGridCount = _gridSystemManager.MainGrid.GetOccupiedCellCount();
            var waitingAreaCount = _waitingAreaController.GetWaitingCharacterCount();

            
            if (mainGridCount == 0 && waitingAreaCount == 0)
            {
                ProcessWinState();
            }
        }
        
        private void CheckForStuckCharacters()
        {
            Debug.Log("Checking for stuck characters...");
            if (!_gameplayStateHolder.IsGameplayActive) return;

            var mainGridCount = _gridSystemManager.MainGrid.GetOccupiedCellCount();
            var waitingAreaCount = _waitingAreaController.GetWaitingCharacterCount();

            if (mainGridCount > 0 || waitingAreaCount > 0)
            {
                ProcessGameOverState();
            }
        }
        
        private void CheckForWaitingAreaDeadlock()
        {
            if (!_gameplayStateHolder.IsGameplayActive) return;
            if (!_waitingAreaController.IsFull()) return;

            var currentBus = _busSystemManager.CurrentBus;
            if (currentBus == null) return;


            var busColor = currentBus.GetColor();
            bool canAnyoneBoard = _waitingAreaController.GetWaitingCharacters().Any(c => c != null && c.Color == busColor);

            if (!canAnyoneBoard)
            {
                ProcessGameOverState();
            }
        }
        
        private void ProcessGameOverState()
        {
            if (!_gameplayStateHolder.IsGameplayActive) return;
            
            _gameplayStateHolder.Pause();
            _timerController.Stop();
            
            _signalBus.Fire<GameOverSignal>();
            Debug.Log("Game Over!");
        }
        
        private void ProcessWinState()
        {
            if (!_gameplayStateHolder.IsGameplayActive) return;

            _gameplayStateHolder.Pause();
            _timerController.Stop();
            _isLevelWon = true;
            Debug.Log("Level Won!");
        }
        
        private void OnNewLevelSequenceRequested()
        {
            _gameplayStateHolder.Resume();
            DOTween.KillAll();
            _movementTracker.Reset();

        }
    }
}