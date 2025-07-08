using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.Core.Grid
{
    public class GridSystemManager : IGridSystemManager
    {
        public IGrid MainGrid { get; }
        public IGrid WaitingAreaGrid { get; }

        public GridSystemManager(DiContainer container, GridConfiguration config)
        {
            var mainGridOrigin = Vector3.zero;
            MainGrid = new Grid(config.MainGridWidth, config.MainGridHeight, mainGridOrigin, config.MainGridTilePrefab, container);
            
            var waitingAreaOrigin = mainGridOrigin + new Vector3(0, 0, config.MainGridHeight + config.SpacingBetweenGrids);
            WaitingAreaGrid = new Grid(config.WaitingGridWidth, config.WaitingGridHeight, waitingAreaOrigin, config.WaitingAreaTilePrefab, container);
        }
    }
}