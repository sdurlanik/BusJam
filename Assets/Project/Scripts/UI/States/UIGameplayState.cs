using UnityEngine;

namespace Sdurlanik.BusJam.UI.States
{
    public class UIGameplayState : IUIState
    {
        private readonly GameObject _panel;

        public UIGameplayState(GameObject panel)
        {
            _panel = panel;
        }

        public void Enter(object payload = null)
        {
            _panel.SetActive(true);
        }

        public void Exit()
        {
            _panel.SetActive(false);
        }
    }
}