using Cysharp.Threading.Tasks;

namespace Sdurlanik.BusJam.Core.Movement
{
    public class MovementTracker : IMovementTracker
    {
        private int _activeMovements = 0;

        public void RegisterMovement()
        {
            _activeMovements++;
        }

        public void UnregisterMovement()
        {
            if (_activeMovements > 0)
            {
                _activeMovements--;
            }
        }

        public async UniTask WaitForAllMovementsToComplete()
        {
            await UniTask.WaitUntil(() => _activeMovements == 0);
        }
        
        public void Reset()
        {
            _activeMovements = 0;
        }
    }
}