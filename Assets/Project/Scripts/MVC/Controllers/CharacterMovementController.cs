using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sdurlanik.BusJam.Core.Grid;
using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.Core.Pathfinding;
using Sdurlanik.BusJam.Core.State;
using Sdurlanik.BusJam.MVC.Views;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.MVC.Controllers
{
    public class CharacterMovementController : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly IGridSystemManager _gridSystemManager;
        private readonly IWaitingAreaController _waitingAreaController;
        private readonly IGameplayStateHolder _gameplayStateHolder;
        private readonly IPathfindingService _pathfindingService;

        public CharacterMovementController(SignalBus signalBus, IGridSystemManager gridSystemManager,
            IWaitingAreaController waitingAreaController, IGameplayStateHolder gameplayStateHolder,
            IPathfindingService pathfindingService)
        {
            _signalBus = signalBus;
            _gridSystemManager = gridSystemManager;
            _waitingAreaController = waitingAreaController;
            _gameplayStateHolder = gameplayStateHolder;
            _pathfindingService = pathfindingService;
        }
        
        public void Initialize() => _signalBus.Subscribe<CharacterClickedSignal>(OnCharacterClicked);
        public void Dispose() => _signalBus.TryUnsubscribe<CharacterClickedSignal>(OnCharacterClicked);

        private void OnCharacterClicked(CharacterClickedSignal signal)
        {
            if (!_gameplayStateHolder.IsGameplayActive) return;

            HandleCharacterMovement(signal.ClickedCharacter).Forget();
        }

        private async UniTask HandleCharacterMovement(CharacterView character)
        {
            if (character.IsMoving) return;

            var path = FindPathToExit(character, _gridSystemManager.MainGrid);
            if (path == null) return;
            
            var reservedSlot = _waitingAreaController.ReserveNextAvailableSlot();
            if (reservedSlot == null) return;

            character.IsMoving = true;
            
            _gridSystemManager.MainGrid.ClearCell(character.GridPosition);
            await character.MoveAlongPath(path);
            await _waitingAreaController.FinalizeMoveToSlot(character, reservedSlot.Value);
            
            character.IsMoving = false;
        }
        
        private List<Vector2Int> FindPathToExit(CharacterView character, IGrid grid)
        {
            var exitPoints = GetExitPoints(grid, character);
            if (exitPoints.Count == 0)
            {
                return null;
            }

            List<Vector2Int> shortestPath = null;
            foreach (var exitPoint in exitPoints)
            {
                var foundPath = _pathfindingService.FindPath(grid, character.GridPosition, exitPoint);
                if (foundPath != null && (shortestPath == null || foundPath.Count < shortestPath.Count))
                {
                    shortestPath = foundPath;
                }
            }

            return (shortestPath != null && shortestPath.Count >= 1) ? shortestPath : null;
        }


        private List<Vector2Int> GetExitPoints(IGrid grid, CharacterView characterToMove)
        {
            var exits = new List<Vector2Int>();
            int topRowIndex = grid.Height - 1;

            for (int x = 0; x < grid.Width; x++)
            {
                var exitPos = new Vector2Int(x, topRowIndex);
                var objectAtExit = grid.GetObjectAt(exitPos);

                if (objectAtExit == null || objectAtExit == characterToMove.gameObject)
                {
                    exits.Add(exitPos);
                }
            }
            return exits;
        }
    }
}