namespace Sdurlanik.BusJam.UI
{
    public interface IUIStateMachine
    {
        void ChangeState<T>(object payload = null) where T : States.IUIState;
    }
}