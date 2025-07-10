using System;
using Sdurlanik.BusJam.Core.Events;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.Core.Grid
{
    public class GridSystemManager : IGridSystemManager, IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        public IGrid MainGrid { get; }
        public IGrid WaitingAreaGrid { get; }
        public void Initialize() => _signalBus.Subscribe<ResetGameplaySignal>(Reset);
        public void Dispose() => _signalBus.TryUnsubscribe<ResetGameplaySignal>(Reset);

        public GridSystemManager(SignalBus signalBus, DiContainer container, GridConfiguration config)
        {
            _signalBus = signalBus;
            var mainGridOrigin = Vector3.zero;
            MainGrid = new Grid(config.MainGridWidth, config.MainGridHeight, mainGridOrigin, config.MainGridTilePrefab, container);
            
            var waitingAreaOrigin = mainGridOrigin + new Vector3(0, 0, config.MainGridHeight + config.SpacingBetweenGrids);
            WaitingAreaGrid = new Grid(config.WaitingGridWidth, config.WaitingGridHeight, waitingAreaOrigin, config.WaitingAreaTilePrefab, container);
        }
        
        public void Reset()
        {
            MainGrid.ClearAllCells();
            WaitingAreaGrid.ClearAllCells();
            Debug.Log("GridSystemManager Reset.");
        }
    }
}