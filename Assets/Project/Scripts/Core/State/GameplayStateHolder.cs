using UnityEngine;

namespace Sdurlanik.BusJam.Core.State
{
    public class GameplayStateHolder : IGameplayStateHolder
    {
        public bool IsGameplayActive { get; private set; } = true;

        public void Pause()
        {
            IsGameplayActive = false;
        }

        public void Resume()
        {
            IsGameplayActive = true;
        }
    }
}