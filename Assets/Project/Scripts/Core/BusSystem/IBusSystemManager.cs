using Sdurlanik.BusJam.MVC.Controllers;

namespace Sdurlanik.BusJam.Core.BusSystem
{
    public interface IBusSystemManager
    {
        IBusController CurrentBus { get; }
        void Reset();
    }
}