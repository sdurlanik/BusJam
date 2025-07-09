using Cysharp.Threading.Tasks;
using Sdurlanik.BusJam.MVC.Views;
using UnityEngine;

namespace Sdurlanik.BusJam.MVC.Controllers
{
    public interface IBusController
    {
        BusView View { get; }
        bool TryBoardCharacter(CharacterView character);
        
        UniTask  Initialize(Vector3 arrivalPosition);
    }
}