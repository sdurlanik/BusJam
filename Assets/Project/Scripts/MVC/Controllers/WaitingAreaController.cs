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
    public class WaitingAreaController : IWaitingAreaController
    {
        private readonly IGridSystemManager _gridSystemManager;
        private readonly IBusSystemManager _busSystemManager;
        private readonly SignalBus _signalBus;
        private readonly CharacterView[] _slots;
        
        public WaitingAreaController(IGridSystemManager gridSystemManager, GridConfiguration gridConfig, IBusSystemManager busSystemManager, SignalBus signalBus)
        {
            _gridSystemManager = gridSystemManager;
            _busSystemManager = busSystemManager;
            _signalBus = signalBus;
            _slots = new CharacterView[gridConfig.WaitingGridWidth];
        }
        
        public void Initialize()
        {
            _signalBus.Subscribe<BusArrivedSignal>(OnBusArrived);
        }
        public bool IsCharacterInArea(CharacterView character)
        {
            return _slots.Contains(character);
        }

        public void RemoveCharacterFromArea(CharacterView character)
        {
            int index = System.Array.IndexOf(_slots, character);
            if (index != -1)
            {
                _slots[index] = null;
                Debug.Log($"Character {character.name} removed from waiting area slot {index}.");
            }
            
        }
        public async UniTask AddCharacterToArea(CharacterView character)
        {
            var waitingGrid = _gridSystemManager.WaitingAreaGrid;
            
          var emptySlotIndex = FindEmptySlotIndex();

            if (emptySlotIndex.HasValue)
            {
                var slotGridPosition = new Vector2Int(emptySlotIndex.Value, 0);
                
                var targetPosition = waitingGrid.GetWorldPosition(slotGridPosition, 0.5f);
                
                await character.MoveToPoint(targetPosition);
                
                waitingGrid.PlaceObject(character.gameObject, slotGridPosition);
                character.UpdateGridPosition(slotGridPosition); 
                _slots[emptySlotIndex.Value] = character; 
                
                Debug.Log($"Character {character.name} placed in waiting area slot {emptySlotIndex.Value}.");
                CheckAndBoardCharacter(character);
            }
            else
            {
                Debug.LogError("Waiting area is full! GAME OVER logic should be triggered here.");
                // TODO: fire GameOver signal or handle full waiting area logic
            }
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
        
        private int? FindEmptySlotIndex()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] == null)
                {
                    return i;
                }
            }
            return null; 
        }
    }
}