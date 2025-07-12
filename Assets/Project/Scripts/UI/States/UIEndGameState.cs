using Sdurlanik.BusJam.UI.Views;
using UnityEngine;

namespace Sdurlanik.BusJam.UI.States
{
    public class UIEndGameState : IUIState
    {
        private readonly EndScreenView _view;

        public UIEndGameState(EndScreenView view)
        {
            _view = view;
        }

        public void Enter(object payload = null)
        {
            _view.gameObject.SetActive(true);
            
            if (payload is bool didWin)
            {
                _view.Show(didWin);
            }
        }

        public void Exit()
        {
            _view.gameObject.SetActive(false);
        }
    }
}