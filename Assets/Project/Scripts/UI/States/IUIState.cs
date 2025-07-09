namespace Sdurlanik.BusJam.UI.States
{
    public interface IUIState
    {
        void Enter(object payload = null);
        void Exit();
    }
}