using System.Collections.Generic;
using System.Linq;
using Sdurlanik.BusJam.Core.Grid;
using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.MVC.Views;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.MVC.Controllers
{
    public class CharacterMovementController
    {
        private readonly SignalBus _signalBus;
        private readonly IGridSystemManager _gridSystemManager;
        private readonly IWaitingAreaController _waitingAreaController;

        public CharacterMovementController(SignalBus signalBus, IGridSystemManager gridSystemManager, IWaitingAreaController waitingAreaController)
        {
            _signalBus = signalBus;
            _gridSystemManager = gridSystemManager;
            _waitingAreaController = waitingAreaController;
            
            _signalBus.Subscribe<CharacterClickedSignal>(OnCharacterClicked);
        }

            private async void OnCharacterClicked(CharacterClickedSignal signal)
        {
            var character = signal.ClickedCharacter;

            if (character.IsMoving)
            {
                return;
            }

            var mainGrid = _gridSystemManager.MainGrid;
            var path = FindPathOnMainGrid(character, mainGrid);
            if (path == null)
            {
                return;
            }
            
            var reservedSlot = _waitingAreaController.ReserveNextAvailableSlot();
            if (reservedSlot == null)
            {
                Debug.LogWarning("Path found on main grid, but no available slot in waiting area to reserve.");
                _signalBus.Fire<GameOverSignal>();
                return;
            }

            character.IsMoving = true;
            try
            {
                var startPos = character.GridPosition;
                
                mainGrid.ClearCell(startPos);
                
                await character.MoveAlongPath(path);
                
                await _waitingAreaController.FinalizeMoveToSlot(character, reservedSlot.Value);
            }
            finally
            {
                character.IsMoving = false;
            }
        }

        private List<Vector2Int> FindPathOnMainGrid(CharacterView character, IGrid grid)
        {
            var exitPoints = GetExitPoints(grid);
            if (exitPoints.Count == 0) return null;

            List<Vector2Int> shortestPath = null;
            foreach (var exitPoint in exitPoints)
            {
                var foundPath = grid.FindPath(character.GridPosition, exitPoint);
                if (foundPath != null && (shortestPath == null || foundPath.Count < shortestPath.Count))
                {
                    shortestPath = foundPath;
                }
            }
            
            return (shortestPath != null && shortestPath.Count > 1) ? shortestPath : null;
        }

        private List<Vector2Int> GetExitPoints(IGrid grid)
        {
            var exits = new List<Vector2Int>();
            int topRow = 4;
            int gridWidth = 5; 
            for (int i = 0; i < gridWidth; i++) 
            {
                var exitPos = new Vector2Int(i, topRow);
                if (grid.IsCellAvailable(exitPos))
                {
                    exits.Add(exitPos);
                }
            }
            return exits;
        }
    }
}