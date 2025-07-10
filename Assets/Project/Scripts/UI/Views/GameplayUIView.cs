using TMPro;
using UnityEngine;
using Zenject;
using Sdurlanik.BusJam.Core.Events;
using System;

namespace Sdurlanik.BusJam.UI.Views
{
    public class GameplayUIView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _timerText;

        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void OnEnable()
        {
            _signalBus.Subscribe<LevelReadySignal>(OnLevelReady);
            _signalBus.Subscribe<TimerUpdatedSignal>(OnTimerUpdated);
        }

        private void OnDisable()
        {
            _signalBus.TryUnsubscribe<LevelReadySignal>(OnLevelReady);
            _signalBus.TryUnsubscribe<TimerUpdatedSignal>(OnTimerUpdated);
        }

        private void OnLevelReady(LevelReadySignal signal)
        {
            // TODO: we need to get the level number from the signal or game state.
            _levelText.text = "LEVEL 1"; 
        }

        private void OnTimerUpdated(TimerUpdatedSignal signal)
        {
            var time = TimeSpan.FromSeconds(signal.RemainingTime);
            _timerText.text = $"{time.Minutes:00}:{time.Seconds:00}";
        }
    }
}