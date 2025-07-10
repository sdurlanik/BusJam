using System;
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
    public class WaitingAreaController : IWaitingAreaController, IInitializable, IDisposable
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
            _signalBus.Subscribe<ResetGameplaySignal>(Reset);
        }
        
        public void Dispose()
        {
            _signalBus.TryUnsubscribe<BusArrivedSignal>(OnBusArrived);
            _signalBus.TryUnsubscribe<ResetGameplaySignal>(Reset);
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
            
            await CheckAndBoardCharacter(character);
        }
        
        private async void OnBusArrived(BusArrivedSignal signal)
        {
            var waitingCharacters = _slots.Where(c => c != null).ToList();
           
            foreach (var character in waitingCharacters)
            {
                if (_busSystemManager.CurrentBus == null || !_busSystemManager.CurrentBus.HasSpace())
                {
                    break; 
                }

                await CheckAndBoardCharacter(character);
            }
        }
        
        private async UniTask<bool> CheckAndBoardCharacter(CharacterView character)
        {
            var currentBus = _busSystemManager.CurrentBus;
            if (currentBus == null) return false;

            if (currentBus.CanBoard(character))
            {
                RemoveCharacterFromArea(character);
        
                await currentBus.BoardCharacterAsync(character);
        
                return true;
            }

            return false;
        }
        
        public int GetWaitingCharacterCount()
        {
            return _slots.Count(character => character != null);
        }
        
        public void Reset()
        {
            if (_slots != null)
            {
                Array.Clear(_slots, 0, _slots.Length);
            }
            
            _reservedSlots.Clear();
        }
        
        public IReadOnlyList<CharacterView> GetWaitingCharacters()
        {
            return _slots;
        }
        
        public bool IsFull()
        {
            return GetWaitingCharacterCount() >= _slots.Length;
        }
    }
}