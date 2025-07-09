using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.Models;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Sdurlanik.BusJam.UI.Views
{
    public class StartScreenView : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private LevelSO _levelToLoad;

        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Start()
        {
            _startButton.onClick.AddListener(OnStartButtonPressed);
        }

        private void OnStartButtonPressed()
        {
            if (_levelToLoad != null)
            {
                _signalBus.Fire(new LevelLoadRequestedSignal(_levelToLoad));
            }
            else
            {
                Debug.LogError("Level to load is not assigned in StartScreenView!");
            }
        }
        
        private void OnDestroy()
        {
            _startButton.onClick.RemoveListener(OnStartButtonPressed);
        }
    }
}