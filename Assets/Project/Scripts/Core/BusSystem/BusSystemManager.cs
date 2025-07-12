using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.Core.Factories;
using Sdurlanik.BusJam.Core.Movement;
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
        public bool IsBusInTransition { get; private set; }
        private IBusController _nextBusInQueue;
        private readonly IGameplayStateHolder _gameplayStateHolder;
        
        private readonly SignalBus _signalBus;
        private readonly IBusFactory _busFactory;
        private readonly IMovementTracker _movementTracker;
        
        private Queue<CharacterColor> _busQueue;
        private Vector3 _busStopPosition;
        private Vector3 _nextBusPosition;
        private bool _isBusDeparting = false;
        
        private CancellationTokenSource _cancellationTokenSource;

        public BusSystemManager(SignalBus signalBus, IBusFactory busFactory, IGameplayStateHolder gameplayStateHolder, IMovementTracker movementTracker)
        {
            _signalBus = signalBus;
            _busFactory = busFactory;
            _gameplayStateHolder = gameplayStateHolder;
            _movementTracker = movementTracker;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<LevelReadySignal>(OnLevelReady);
            _signalBus.Subscribe<BusFullSignal>(OnBusFull);
            _signalBus.Subscribe<ResetGameplaySignal>(Reset);
            
            _cancellationTokenSource = new CancellationTokenSource();
        }
        
        public void Dispose()
        {
            _signalBus.TryUnsubscribe<LevelReadySignal>(OnLevelReady);
            _signalBus.TryUnsubscribe<BusFullSignal>(OnBusFull);
            _signalBus.TryUnsubscribe<ResetGameplaySignal>(Reset);
            
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
        
        private void OnLevelReady(LevelReadySignal signal)
        {
            _busQueue = new Queue<CharacterColor>(signal.LevelData.BusColorSequence);
            _busStopPosition = signal.LevelData.BusStopPosition;
            _nextBusPosition = _busStopPosition + signal.LevelData.NextBusOffset;
        
            SpawnInitialBusesAsync(_cancellationTokenSource.Token).Forget();
        }
        
        private void OnBusFull(BusFullSignal signal)
        {
            if (!_gameplayStateHolder.IsGameplayActive || signal.FullBus != CurrentBus) return;
            ProcessBusDepartureAsync(_cancellationTokenSource.Token).Forget();
        }

        private async UniTask ProcessBusDepartureAsync(CancellationToken cancellationToken)
        {
            IsBusInTransition = true;
            var departingBus = CurrentBus;
            
            CurrentBus = _nextBusInQueue;
            _nextBusInQueue = null;

            _movementTracker.RegisterMovement();
            try
            {
                await departingBus.View.AnimateDeparture(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Bus departure was canceled by Reset.");
            }
            finally
            {
                _movementTracker.UnregisterMovement();
                if (departingBus.View != null)
                {
                    Object.Destroy(departingBus.View.gameObject);
                }
            }
            
            if (cancellationToken.IsCancellationRequested)
            {
                IsBusInTransition = false;
                return;
            }

            if (CurrentBus != null)
            {
                _signalBus.Fire(new BusArrivalSequenceStartedSignal(CurrentBus));
                await CurrentBus.View.AnimateToStopPosition(_busStopPosition,cancellationToken);
                CurrentBus.IsAcceptingPassengers = true;
                IsBusInTransition = false;
                _signalBus.Fire(new BusArrivedSignal(CurrentBus));
                
                if (_busQueue.Count > 0)
                {
                    var nextColor = _busQueue.Dequeue();
                    _nextBusInQueue = _busFactory.CreateAtPosition(nextColor, _nextBusPosition);
                    _nextBusInQueue.View.AnimateArrival(_nextBusPosition, cancellationToken).Forget();
                }
            }
            else
            {
                IsBusInTransition = false;
                _signalBus.Fire<AllBusesDispatchedSignal>();
            }
        }
        
        public void Reset()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            
            _cancellationTokenSource = new CancellationTokenSource();

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
        
        private async UniTask SpawnInitialBusesAsync(CancellationToken cancellationToken)
        {
            if (_busQueue.Count > 0)
            {
                var color = _busQueue.Dequeue();
        
                CurrentBus = await _busFactory.Create(color, _busStopPosition, cancellationToken);
        
                if (CurrentBus != null)
                {
                    CurrentBus.IsAcceptingPassengers = true;
                }
                
                if (cancellationToken.IsCancellationRequested) return;
                _signalBus.Fire(new BusArrivedSignal(CurrentBus));
            }

            if (_busQueue.Count > 0)
            {
                var color = _busQueue.Dequeue();
                _nextBusInQueue = _busFactory.CreateAtPosition(color, _nextBusPosition);
                _nextBusInQueue.View.AnimateArrival(_nextBusPosition, cancellationToken).Forget();
            }
        }
    }
}