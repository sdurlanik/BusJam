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
            _signalBus.Fire<StartGameRequestedSignal>();
        }
        
        private void OnDestroy()
        {
            _startButton.onClick.RemoveListener(OnStartButtonPressed);
        }
    }
}