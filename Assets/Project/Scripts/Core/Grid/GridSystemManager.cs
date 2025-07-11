using System;
using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.Models;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.Core.Grid
{
    public class GridSystemManager : IGridSystemManager, IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly GridConfiguration _config;
        private readonly DiContainer _container;
        private bool _areGridsCreated = false;
        public IGrid MainGrid { get; private set; }
        public IGrid WaitingAreaGrid { get; private set; }
        public void Initialize() => _signalBus.Subscribe<ResetGameplaySignal>(Reset);
        public void Dispose() => _signalBus.TryUnsubscribe<ResetGameplaySignal>(Reset);

        public GridSystemManager(SignalBus signalBus, DiContainer container, GridConfiguration config)
        {
            _signalBus = signalBus;
            _container = container;
            _config = config;
        }

        public void CreateGrids(LevelSO levelData)
        {
            if (_areGridsCreated) return;

            var mainGridOrigin = Vector3.zero;
            MainGrid = new Grid(levelData.MainGridSize.x, levelData.MainGridSize.y, mainGridOrigin,
                _config.MainGridTilePrefab, _container);

            var waitingAreaOrigin = mainGridOrigin + new Vector3(0, 0, levelData.MainGridSize.y + _config.SpacingBetweenGrids);
            WaitingAreaGrid = new Grid(_config.WaitingGridSize.x, _config.WaitingGridSize.y, waitingAreaOrigin,
                _config.WaitingAreaTilePrefab, _container);

            _areGridsCreated = true;
        }

        public void Reset()
        {
            if (!_areGridsCreated) return;

            MainGrid.ClearAllCells();
            WaitingAreaGrid.ClearAllCells();

            MainGrid = null;
            WaitingAreaGrid = null;
            _areGridsCreated = false;

            Debug.Log("Grids have been destroyed and reset.");
        }
    }
}