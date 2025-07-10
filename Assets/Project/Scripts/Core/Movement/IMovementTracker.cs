using Cysharp.Threading.Tasks;

namespace Sdurlanik.BusJam.Core.Movement
{
    public interface IMovementTracker
    {
        void RegisterMovement();
        void UnregisterMovement();
        UniTask WaitForAllMovementsToComplete();
        void Reset();
    }
}