using System.Collections.Generic;
using System.Linq;
using Sdurlanik.BusJam.Core;
using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.MVC.Views;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.MVC.Controllers
{
    public class CharacterMovementController
    {
        private readonly SignalBus _signalBus;
        private readonly IGridManager _gridManager;

        public CharacterMovementController(SignalBus signalBus, IGridManager gridManager)
        {
            _signalBus = signalBus;
            _gridManager = gridManager;
            
            _signalBus.Subscribe<CharacterClickedSignal>(OnCharacterClicked);
        }

        private async void OnCharacterClicked(CharacterClickedSignal signal)
        {
            var character = signal.ClickedCharacter;
            var startPos = character.GridPosition;
            Debug.Log($"MovementController received click for character at {character.GridPosition}. Checking path...");

            var exitPoints = GetExitPoints();
            List<Vector2Int> shortestPath = null;

            foreach (var exitPoint in exitPoints)
            {
                var foundPath = _gridManager.FindPath(character.GridPosition, exitPoint);

                if (foundPath != null)
                {
                    if (shortestPath == null || foundPath.Count < shortestPath.Count)
                    {
                        shortestPath = foundPath;
                    }
                }
            }

            if (shortestPath != null)
            {
                string pathString = string.Join(" -> ", shortestPath.Select(p => p.ToString()));
                Debug.Log($"Path FOUND for character {character.name}. Path: {pathString}. Starting movement...");
             
                _gridManager.ClearCell(startPos);
                await character.MoveAlongPath(shortestPath);
                var endPos = shortestPath.Last();
                
                Debug.Log($"Character {character.name} has finished its movement.");
                _gridManager.PlaceObject(character.gameObject, endPos);
                character.UpdateGridPosition(endPos);
                // TODO: Place character at the waiting cell
            }
            else
            {
                Debug.LogWarning($"No valid path to any exit point found for character {character.name}.");
            }
        }

        private List<Vector2Int> GetExitPoints()
        {
            var exits = new List<Vector2Int>();
            int topRow = 4;
            for (int i = 0; i < 5; i++) 
            {
                var exitPos = new Vector2Int(i, topRow);
                if (_gridManager.IsCellAvailable(exitPos))
                {
                    exits.Add(exitPos);
                }
            }
            return exits;
        }
    }
}