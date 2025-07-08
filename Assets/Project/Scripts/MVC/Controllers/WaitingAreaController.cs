using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sdurlanik.BusJam.Core.Grid;
using Sdurlanik.BusJam.MVC.Views;
using UnityEngine;

namespace Sdurlanik.BusJam.MVC.Controllers
{
    public class WaitingAreaController : IWaitingAreaController
    {
        private readonly IGridSystemManager _gridSystemManager;
        private readonly CharacterView[] _slots;
        
        public WaitingAreaController(IGridSystemManager gridSystemManager, GridConfiguration gridConfig)
        {
            _gridSystemManager = gridSystemManager;
            _slots = new CharacterView[gridConfig.WaitingGridWidth];
        }
        public async UniTask AddCharacterToArea(CharacterView character)
        {
            var waitingGrid = _gridSystemManager.WaitingAreaGrid;
            
            int? emptySlotIndex = FindEmptySlotIndex();

            if (emptySlotIndex.HasValue)
            {
                var slotGridPosition = new Vector2Int(emptySlotIndex.Value, 0);
                
                var targetPosition = waitingGrid.GetWorldPosition(slotGridPosition, 0.5f);
                
                await character.MoveToPoint(targetPosition);
                
                waitingGrid.PlaceObject(character.gameObject, slotGridPosition);
                character.UpdateGridPosition(slotGridPosition); 
                _slots[emptySlotIndex.Value] = character; 
                
                Debug.Log($"Character {character.name} placed in waiting area slot {emptySlotIndex.Value}.");
            }
            else
            {
                Debug.LogError("Waiting area is full! GAME OVER logic should be triggered here.");
                // TODO: fire GameOver signal or handle full waiting area logic
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