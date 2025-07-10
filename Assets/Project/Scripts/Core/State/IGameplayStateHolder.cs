namespace Sdurlanik.BusJam.Core.State
{
    public interface IGameplayStateHolder
    {
        bool IsGameplayActive { get; }
        void Pause();
        void Resume();
    }
}