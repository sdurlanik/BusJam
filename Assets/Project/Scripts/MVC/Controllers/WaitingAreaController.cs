using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sdurlanik.BusJam.Core.Grid;
using Sdurlanik.BusJam.MVC.Views;
using UnityEngine;
using System.Linq;
using Sdurlanik.BusJam.Core.BusSystem;
using Sdurlanik.BusJam.Core.Events;
using Zenject;

namespace Sdurlanik.BusJam.MVC.Controllers
{
    public class WaitingAreaController : IWaitingAreaController, IInitializable
    {
        private readonly IGridSystemManager _gridSystemManager;
        private readonly IBusSystemManager _busSystemManager;
        private readonly SignalBus _signalBus;
        private readonly CharacterView[] _slots;
        
        private readonly HashSet<Vector2Int> _reservedSlots;
        
        public WaitingAreaController(IGridSystemManager gridSystemManager, GridConfiguration gridConfig, IBusSystemManager busSystemManager, SignalBus signalBus)
        {
            _gridSystemManager = gridSystemManager;
            _busSystemManager = busSystemManager;
            _signalBus = signalBus;
            _slots = new CharacterView[gridConfig.WaitingGridWidth];
            _reservedSlots = new HashSet<Vector2Int>();
        }
        
        public void Initialize()
        {
            _signalBus.Subscribe<BusArrivedSignal>(OnBusArrived);
        }
        public bool IsCharacterInArea(CharacterView character)
        {
            return _slots.Contains(character);
        }

        public Vector2Int? ReserveNextAvailableSlot()
        {
            var grid = _gridSystemManager.WaitingAreaGrid;
            for (int x = 0; x < _slots.Length; x++)
            {
                var cell = new Vector2Int(x, 0);
                if (grid.IsCellAvailable(cell) && !_reservedSlots.Contains(cell))
                {
                    _reservedSlots.Add(cell);
                    Debug.Log($"Waiting area slot {cell} has been reserved.");
                    return cell;
                }
            }
            return null;
        }
        public void RemoveCharacterFromArea(CharacterView character)
        {
            int index = System.Array.IndexOf(_slots, character);
            if (index != -1)
            {
                var gridPos = new Vector2Int(index, 0);
                _gridSystemManager.WaitingAreaGrid.ClearCell(gridPos);
                _slots[index] = null;
                Debug.Log($"Character {character.name} removed from waiting area slot {index}.");
            }
        }
        public async UniTask FinalizeMoveToSlot(CharacterView character, Vector2Int reservedSlot)
        {
            var waitingGrid = _gridSystemManager.WaitingAreaGrid;
            var targetPosition = waitingGrid.GetWorldPosition(reservedSlot, 0.5f);
            
            await character.MoveToPoint(targetPosition);

            _reservedSlots.Remove(reservedSlot);
            waitingGrid.PlaceObject(character.gameObject, reservedSlot);
            _slots[reservedSlot.x] = character;
            character.UpdateGridPosition(reservedSlot);
            
            Debug.Log($"Character {character.name} finalized move to waiting area slot {reservedSlot}.");

            CheckAndBoardCharacter(character);
        }
        
        private void OnBusArrived(BusArrivedSignal signal)
        {
            Debug.Log("New bus arrived, checking waiting area for passengers...");
            var waitingCharacters = _slots.Where(c => c != null).ToList();
            foreach (var character in waitingCharacters)
            {
                CheckAndBoardCharacter(character);
            }
        }
        
        private void CheckAndBoardCharacter(CharacterView character)
        {
            var currentBus = _busSystemManager.CurrentBus;
            if (currentBus == null) return;

            bool success = currentBus.TryBoardCharacter(character);
            if (success)
            {
                RemoveCharacterFromArea(character);
            }
        }
    }
}