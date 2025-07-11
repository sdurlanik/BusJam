using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.Core.Factories;
using Sdurlanik.BusJam.Core.State;
using Sdurlanik.BusJam.Models;
using Sdurlanik.BusJam.MVC.Controllers;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Sdurlanik.BusJam.Core.BusSystem
{
    public class BusSystemManager : IBusSystemManager, IInitializable, IDisposable
    {
        public IBusController CurrentBus { get; private set; }
        private IBusController _nextBusInQueue;
        private readonly IGameplayStateHolder _gameplayStateHolder;
        
        private readonly SignalBus _signalBus;
        private readonly IBusFactory _busFactory;
        
        private Queue<CharacterColor> _busQueue;
        private Vector3 _busStopPosition;
        private Vector3 _nextBusPosition;
        private bool _isBusDeparting = false;

        public BusSystemManager(SignalBus signalBus, IBusFactory busFactory, IGameplayStateHolder gameplayStateHolder)
        {
            _signalBus = signalBus;
            _busFactory = busFactory;
            _gameplayStateHolder = gameplayStateHolder;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<LevelReadySignal>(OnLevelReady);
            _signalBus.Subscribe<BusFullSignal>(OnBusFull);
            _signalBus.Subscribe<ResetGameplaySignal>(Reset);
        }
        
        public void Dispose()
        {
            _signalBus.TryUnsubscribe<LevelReadySignal>(OnLevelReady);
            _signalBus.TryUnsubscribe<BusFullSignal>(OnBusFull);
            _signalBus.TryUnsubscribe<ResetGameplaySignal>(Reset);
        }
        
        private void OnLevelReady(LevelReadySignal signal)
        {
            _busQueue = new Queue<CharacterColor>(signal.LevelData.BusColorSequence);
            _busStopPosition = signal.LevelData.BusStopPosition;
            _nextBusPosition = _busStopPosition + signal.LevelData.NextBusOffset;
        
            SpawnInitialBuses();
        }
        
        private async void OnBusFull(BusFullSignal signal)
        {
            if (!_gameplayStateHolder.IsGameplayActive || signal.FullBus != CurrentBus) return;
        
            await CurrentBus.View.AnimateDeparture();
        
            CurrentBus = _nextBusInQueue;
            _nextBusInQueue = null;

            if (_busQueue.Count > 0)
            {
                var nextColor = _busQueue.Dequeue();
                _nextBusInQueue = _busFactory.CreateAtPosition(nextColor, _nextBusPosition);
            }
        
            if (CurrentBus != null)
            {
                await CurrentBus.View.AnimateToStopPosition(_busStopPosition);
                _signalBus.Fire(new BusArrivedSignal(CurrentBus));
            }
            else
            {
                _signalBus.Fire<AllBusesDispatchedSignal>();
            }
        }
        
        public void Reset()
        {
            if (CurrentBus != null && CurrentBus.View != null)
            {
                Object.Destroy(CurrentBus.View.gameObject);
            }
            if (_nextBusInQueue != null && _nextBusInQueue.View != null)
            {
                Object.Destroy(_nextBusInQueue.View.gameObject);
            }
            CurrentBus = null;
            _nextBusInQueue = null;
            _busQueue?.Clear();
        }
        
        private async void SpawnInitialBuses()
        {
            if (_busQueue.Count > 0)
            {
                var color = _busQueue.Dequeue();
                CurrentBus = await _busFactory.Create(color, _busStopPosition);
                _signalBus.Fire(new BusArrivedSignal(CurrentBus));
            }

            if (_busQueue.Count > 0)
            {
                var color = _busQueue.Dequeue();
                _nextBusInQueue = _busFactory.CreateAtPosition(color, _nextBusPosition);
            }
        }
    }
}