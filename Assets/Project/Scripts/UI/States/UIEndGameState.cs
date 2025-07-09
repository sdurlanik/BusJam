using Sdurlanik.BusJam.UI.Views;
using UnityEngine;

namespace Sdurlanik.BusJam.UI.States
{
    public class UIEndGameState : IUIState
    {
        private readonly GameObject _panel;
        private readonly EndScreenView _view;

        public UIEndGameState(GameObject panel)
        {
            _panel = panel;
            _view = panel.GetComponent<EndScreenView>();
        }

        public void Enter(object payload = null)
        {
            _panel.SetActive(true);
            
            if (payload is bool didWin)
            {
                _view.Show(didWin);
            }
        }

        public void Exit()
        {
            _panel.SetActive(false);
        }
    }
}