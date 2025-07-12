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
        public BusView View { get; }
        public bool IsAcceptingPassengers { get; set; }

        private readonly SignalBus _signalBus;
        private readonly BusModel _busModel;
        private readonly IGameplayStateHolder _gameplayStateHolder;

        public BusController(BusModel model, BusView view, SignalBus signalBus, IGameplayStateHolder gameplayStateHolder)
        {
            _busModel = model;
            View = view;
            _signalBus = signalBus;
            _gameplayStateHolder = gameplayStateHolder;
            
            View.SetColor(ColorMapper.GetColorFromEnum(_busModel.BusColor));
        }
        
        public async UniTask Initialize(Vector3 arrivalPosition, CancellationToken cancellationToken)
        {
            await View.AnimateArrival(arrivalPosition, cancellationToken);
        }

        public bool HasSpace()
        {
            return _busModel.HasSpace();
        }

        public CharacterColor GetColor()
        {
            return _busModel.BusColor;
        }
        
        public bool CanBoard(CharacterView character)
        {
            return IsAcceptingPassengers && _busModel.HasSpace() && _busModel.IsColorMatch(character.Color);
        }
        
        public async UniTask BoardCharacterAsync(CharacterView character)
        {
            var slotIndex = _busModel.Passengers.Count;
            var slotTransform = View.GetSlotTransform(slotIndex);

            _busModel.AddPassenger(character);
            
            try
            {
                await character.MoveToPoint(slotTransform.position);
            }
            catch (System.OperationCanceledException)
            {
                // do nothing because the game is already over
            }
            
            if (character != null)
            {
                character.transform.SetParent(slotTransform);
            }

            if (!_busModel.HasSpace() && _gameplayStateHolder.IsGameplayActive)
            {
                _signalBus.Fire(new BusFullSignal(this));
            }
        }
    }
}