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

namespace Sdurlanik.BusJam.Core
{
    public class GameStateManager : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly IGridSystemManager _gridSystemManager;
        private readonly IBusSystemManager _busSystemManager;
        private readonly IWaitingAreaController _waitingAreaController;
        private readonly IMovementTracker _movementTracker;
        
        private bool _isLevelWon = false;
        
        public GameStateManager(SignalBus signalBus, IGridSystemManager gridSystemManager, IWaitingAreaController waitingAreaController, IMovementTracker movementTracker, IBusSystemManager busSystemManager)
        {
            _signalBus = signalBus;
            _gridSystemManager = gridSystemManager;
            _waitingAreaController = waitingAreaController;
            _movementTracker = movementTracker;
            _busSystemManager = busSystemManager;
        }

        public void Initialize()
        {
            _isLevelWon = false;
            
            _signalBus.Subscribe<GameOverSignal>(OnGameOver);
            _signalBus.Subscribe<LevelSuccessSignal>(OnLevelSuccess);
            _signalBus.Subscribe<BusFullSignal>(OnBusFull);
            _signalBus.Subscribe<AllBusesDispatchedSignal>(OnAllBusesDispatched);
            _signalBus.Subscribe<RestartLevelRequestedSignal>(OnRestartLevelRequested);
            _signalBus.Subscribe<BusArrivedSignal>(OnBusArrived);
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<GameOverSignal>(OnGameOver);
            _signalBus.TryUnsubscribe<LevelSuccessSignal>(OnLevelSuccess);
            _signalBus.TryUnsubscribe<BusFullSignal>(OnBusFull);
            _signalBus.TryUnsubscribe<AllBusesDispatchedSignal>(OnAllBusesDispatched);
            _signalBus.TryUnsubscribe<RestartLevelRequestedSignal>(OnRestartLevelRequested);
            _signalBus.TryUnsubscribe<BusArrivedSignal>(OnBusArrived);
        }

        private void OnBusFull() => CheckWinCondition();

        private void OnBusArrived() => CheckForWaitingAreaDeadlock();
        
        private void OnAllBusesDispatched()
        {
            if (_isLevelWon)
            {
                _signalBus.Fire<LevelCompleteSequenceFinishedSignal>();
                return;
            }
            
            CheckForStuckCharactersAsync();  
        } 
        
        private async void CheckWinCondition()
        {
            await UniTask.Yield();
            
            int mainGridCount = _gridSystemManager.MainGrid.GetOccupiedCellCount();
            int waitingAreaCount = _waitingAreaController.GetWaitingCharacterCount();

            if (mainGridCount == 0 && waitingAreaCount == 0)
            {
                if (_isLevelWon) return;
                
                _isLevelWon = true;
                _signalBus.Fire<LevelSuccessSignal>();
            }
        }
        
        private async  void CheckForStuckCharactersAsync()
        {
            await UniTask.Yield();

            int mainGridCount = _gridSystemManager.MainGrid.GetOccupiedCellCount();
            int waitingAreaCount = _waitingAreaController.GetWaitingCharacterCount();

            if (mainGridCount > 0 || waitingAreaCount > 0)
            {
                _signalBus.Fire<GameOverSignal>();
            }
        }
        
        private void CheckForWaitingAreaDeadlock()
        {
        
            if (!_waitingAreaController.IsFull()) return;

            var currentBus = _busSystemManager.CurrentBus;
            if (currentBus == null) return;
            
            var busColor = currentBus.GetColor();
            bool canAnyoneBoard = _waitingAreaController.GetWaitingCharacters().Any(c => c != null && c.Color == busColor);

            if (!canAnyoneBoard)
            {
                Debug.LogWarning("Deadlock: Waiting area is full and no character can board. GAME OVER.");
                _signalBus.Fire<GameOverSignal>();
            }
        }
        
        private void OnGameOver()
        {
            Debug.Log("<color=red>GAME OVER! - Game is paused.</color>");
        }
        
        private void OnLevelSuccess()
        {
            Debug.Log("<color=green>LEVEL COMPLETE! - Game is paused.</color>");
        }
        
        private void OnRestartLevelRequested()
        {
            DOTween.KillAll();
            _movementTracker.Reset();
            Debug.Log("GameStateManager: Restart request received.");
        }
    }
}