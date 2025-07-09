using UnityEngine;

namespace Sdurlanik.BusJam.UI.States
{
    public class UIStartState : IUIState
    {
        private readonly GameObject _panel;

        public UIStartState(GameObject panel)
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