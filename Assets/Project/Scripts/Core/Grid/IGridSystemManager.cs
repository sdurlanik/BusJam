namespace Sdurlanik.BusJam.Core.Grid
{
    public interface IGridSystemManager
    {
        IGrid MainGrid { get; }
        IGrid WaitingAreaGrid { get; }
    }
}