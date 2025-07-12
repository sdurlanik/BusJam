using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.Core.State;
using Sdurlanik.BusJam.Models;
using Sdurlanik.BusJam.MVC.Models;
using Sdurlanik.BusJam.MVC.Views;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.MVC.Controllers
{
    public class BusController : IBusController
    {
        private readonly SignalBus _signalBus;
        public BusView View { get; }
        private readonly BusModel _model;
        private readonly IGameplayStateHolder _gameplayStateHolder;
        private readonly List<UniTask> _boardingTasks = new();
        public bool IsAcceptingPassengers { get; set; } = false;
        public BusController(BusModel model, BusView view, SignalBus signalBus, IGameplayStateHolder gameplayStateHolder)
        {
            _model = model;
            View = view;
            _signalBus = signalBus;
            _gameplayStateHolder = gameplayStateHolder;
            
            View.SetColor(ColorMapper.GetColorFromEnum(_model.BusColor));
        }
        
        public async UniTask Initialize(Vector3 arrivalPosition, CancellationToken cancellationToken)
        {
            await View.AnimateArrival(arrivalPosition, cancellationToken);
        }
        
        public bool HasSpace()
        {
            return _model.HasSpace();
        }

        public CharacterColor GetColor()
        {
            return _model.BusColor;
        }
        
        public bool CanBoard(CharacterView character)
        {
            return IsAcceptingPassengers && _model.HasSpace() && _model.IsColorMatch(character.Color);
        }
        
        public async UniTask BoardCharacterAsync(CharacterView character)
        {
            var slotIndex = _model.Passengers.Count;
            var slotTransform = View.GetSlotTransform(slotIndex);

            _model.AddPassenger(character);
        
            var boardingTask = BoardingSequence(character, slotTransform);
            _boardingTasks.Add(boardingTask);
        
            await boardingTask;

            _boardingTasks.Remove(boardingTask);

            character.transform.SetParent(slotTransform);

            if (!_model.HasSpace())
            {
                if (!_gameplayStateHolder.IsGameplayActive) return;
            
                _signalBus.Fire(new BusFullSignal(this));
                Debug.Log("Bus full signal fired");
            }
        }

        private async UniTask BoardingSequence(CharacterView character, Transform slotTransform)
        {
            try
            {
                await character.MoveToPoint(slotTransform.position);
            }
            catch (OperationCanceledException e)
            {
                Debug.LogWarning($"MoveToPoint canceled: {e.Message}");
            }
        }
        public IEnumerable<UniTask> GetPendingBoardingTasks()
        {
            return _boardingTasks;
        }

    }
}