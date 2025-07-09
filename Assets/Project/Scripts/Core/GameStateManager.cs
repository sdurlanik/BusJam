using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.Core.Grid;
using Sdurlanik.BusJam.MVC.Controllers;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using System;

namespace Sdurlanik.BusJam.Core
{
    public class GameStateManager : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly IGridSystemManager _gridSystemManager;
        private readonly IWaitingAreaController _waitingAreaController;

        public GameStateManager(SignalBus signalBus, IGridSystemManager gridSystemManager, IWaitingAreaController waitingAreaController)
        {
            _signalBus = signalBus;
            _gridSystemManager = gridSystemManager;
            _waitingAreaController = waitingAreaController;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<GameOverSignal>(OnGameOver);
            _signalBus.Subscribe<LevelSuccessSignal>(OnLevelSuccess);
            _signalBus.Subscribe<BusFullSignal>(OnBusFull);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<GameOverSignal>(OnGameOver);
            _signalBus.Unsubscribe<LevelSuccessSignal>(OnLevelSuccess);
            _signalBus.Unsubscribe<BusFullSignal>(OnBusFull);
        }

        private void OnBusFull()
        {
            CheckWinConditionAsync();
        }

        private async void CheckWinConditionAsync()
        {
            await UniTask.Yield(); 
            
            var mainGridCount = _gridSystemManager.MainGrid.GetOccupiedCellCount();
            var waitingAreaCount = _waitingAreaController.GetWaitingCharacterCount();

            if (mainGridCount == 0 && waitingAreaCount == 0)
            {
                _signalBus.Fire<LevelSuccessSignal>();
            }
        }

        private void OnGameOver()
        {
            Debug.Log("<color=red>GAME OVER! - Game is paused.</color>");
            // TODO: "Game Over" UI panel will be activated here.
        }

        private void OnLevelSuccess()
        {
            Debug.Log("<color=green>LEVEL COMPLETE! - Game is paused.</color>");
            // TODO: "Tebrikler!" UI panel will be activated here.
        }
    }
}