using Sdurlanik.BusJam.Models;

namespace Sdurlanik.BusJam.Core.Grid
{
    public interface IGridSystemManager
    {
        IGrid MainGrid { get; }
        IGrid WaitingAreaGrid { get; }
        void CreateGrids(LevelSO levelData); 
        void Reset();
    }
}