using System.Collections.Generic;
using System.Linq;
using Sdurlanik.BusJam.Core.Grid;
using Sdurlanik.BusJam.Core.Events;
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
            var startPos = character.GridPosition;
            
            var mainGrid = _gridSystemManager.MainGrid;

            var exitPoints = GetExitPoints(mainGrid);
            List<Vector2Int> shortestPath = null;

            foreach (var exitPoint in exitPoints)
            {
                var foundPath = mainGrid.FindPath(character.GridPosition, exitPoint);

                if (foundPath != null && (shortestPath == null || foundPath.Count < shortestPath.Count))
                {
                    shortestPath = foundPath;
                }
            }

            if (shortestPath != null && shortestPath.Count > 1)
            {
                mainGrid.ClearCell(startPos);
                await character.MoveAlongPath(shortestPath);
                Debug.Log($"Character {character.name} reached exit point. Handing over to WaitingAreaController.");

                await _waitingAreaController.AddCharacterToArea(character);
            }
            else
            {
                Debug.LogWarning($"No valid path to any exit point found for character {character.name}.");
            }
        }

        private List<Vector2Int> GetExitPoints(IGrid grid)
        {
            //TODO: Make this configurable with a grid configuration
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