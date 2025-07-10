using Sdurlanik.BusJam.Core.Events;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Sdurlanik.BusJam.UI.Views
{
    public class EndScreenView : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject _winPanel;
        [SerializeField] private GameObject _losePanel;

        [Header("Buttons")]
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _restartButton;
        
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void OnEnable()
        {
            _nextLevelButton.onClick.AddListener(OnNextLevelPressed);
            _restartButton.onClick.AddListener(OnRestartPressed);
        }

        private void OnDisable()
        {
            _nextLevelButton.onClick.RemoveListener(OnNextLevelPressed);
            _restartButton.onClick.RemoveListener(OnRestartPressed);
        }

        public void Show(bool didWin)
        {
            _winPanel.SetActive(didWin);
            _losePanel.SetActive(!didWin);
            _nextLevelButton.gameObject.SetActive(didWin);
            _restartButton.gameObject.SetActive(!didWin);
        }
        
        private void OnNextLevelPressed()
        {
            Debug.Log("Next Level button pressed.");
            _signalBus.Fire<NextLevelRequestedSignal>();
        }

        private void OnRestartPressed()
        {
            Debug.Log("--- Restart Button Pressed: Firing RestartLevelRequestedSignal ---");
            _signalBus.Fire<RestartLevelRequestedSignal>();
        }
    }
}