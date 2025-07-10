using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.Core.Factories;
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
        
        private readonly SignalBus _signalBus;
        private readonly IBusFactory _busFactory;
        
        private Queue<CharacterColor> _busQueue;
        private Vector3 _busStopPosition;

        public BusSystemManager(SignalBus signalBus, IBusFactory busFactory)
        {
            _signalBus = signalBus;
            _busFactory = busFactory;
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
            SpawnNextBus();
        }

        private async void SpawnNextBus()
        {
            if (_busQueue.Count > 0)
            {
                var nextColor = _busQueue.Dequeue();
                CurrentBus = await _busFactory.Create(nextColor, _busStopPosition);
                _signalBus.Fire(new BusArrivedSignal(CurrentBus));
            }
            else
            {
                Debug.Log("No more buses in the queue. Level might be complete.");
                _signalBus.Fire<AllBusesDispatchedSignal>();            }
        }
        
        private async void OnBusFull(BusFullSignal signal)
        {
            if (signal.FullBus == CurrentBus)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1));
                await CurrentBus.View.AnimateDeparture();
                CurrentBus = null;
            
                SpawnNextBus();
            }
        }
        
        public void Reset()
        {
            if (CurrentBus != null && CurrentBus.View != null)
            {
                Object.Destroy(CurrentBus.View.gameObject);
            }
            CurrentBus = null;
            _busQueue?.Clear();
            Debug.Log("BusSystemManager Reset.");
        }
    }
}