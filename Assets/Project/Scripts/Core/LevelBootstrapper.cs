using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.Models;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.Core
{
    public class LevelBootstrapper : MonoBehaviour
    {
        [Header("Level Data")]
        [SerializeField] private LevelSO _startingLevel;
        
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Start()
        {
            if (_startingLevel == null)
            {
                Debug.LogError("Starting Level is not assigned in the LevelBootstrapper!", this);
                return;
            }
            
            Debug.Log($"Bootstrapper is firing LevelLoadRequestedSignal for level: {_startingLevel.name}");
            _signalBus.Fire(new LevelLoadRequestedSignal(_startingLevel));
        }
    }
}