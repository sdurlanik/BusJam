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

        public BusController(BusModel model, BusView view, SignalBus signalBus, IGameplayStateHolder gameplayStateHolder)
        {
            _model = model;
            View = view;
            _signalBus = signalBus;
            _gameplayStateHolder = gameplayStateHolder;
        }
        
        public async UniTask Initialize(Vector3 arrivalPosition)
        {
            View.SetColor(GetColorFromEnum(_model.BusColor));
            
            await View.AnimateArrival(arrivalPosition);
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
            return _model.HasSpace() && _model.IsColorMatch(character.Color);
        }
        
        public  async UniTask BoardCharacterAsync(CharacterView character)
        {
            var slotIndex = _model.Passengers.Count;
            var slotTransform = View.GetSlotTransform(slotIndex);
            
            _model.AddPassenger(character);
            
            await character.MoveToPoint(slotTransform.position);
            character.transform.SetParent(slotTransform);
            
            if (!_model.HasSpace())
            {
                if (!_gameplayStateHolder.IsGameplayActive) return;
                _signalBus.Fire(new BusFullSignal(this));
                Debug.Log("Bus full signal fired");
            }
        }
        
        private Color GetColorFromEnum(CharacterColor color)
        {
            switch (color)
            {
                case CharacterColor.Red: return Color.red;
                case CharacterColor.Green: return Color.green;
                case CharacterColor.Blue: return Color.blue;
                case CharacterColor.Yellow: return Color.yellow;
                case CharacterColor.Purple: return new Color(0.5f, 0, 0.5f);
                default: return Color.white;
            }
        }
    }
}