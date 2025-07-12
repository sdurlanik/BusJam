using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sdurlanik.BusJam.MVC.Views;
using UnityEngine;

namespace Sdurlanik.BusJam.MVC.Controllers
{
    public interface IWaitingAreaController
    {
        Vector2Int? ReserveNextAvailableSlot();
        UniTask FinalizeMoveToSlot(CharacterView character, Vector2Int reservedSlot);
        int GetWaitingCharacterCount();
        void Reset();
        bool IsFull();
        IReadOnlyList<CharacterView> GetWaitingCharacters();
    }
}