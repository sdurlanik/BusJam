using Cysharp.Threading.Tasks;
using Sdurlanik.BusJam.Models;
using Sdurlanik.BusJam.MVC.Views;
using UnityEngine;

namespace Sdurlanik.BusJam.MVC.Controllers
{
    public interface IBusController
    {
        BusView View { get; }
        bool CanBoard(CharacterView character);
        UniTask BoardCharacterAsync(CharacterView character);
        UniTask  Initialize(Vector3 arrivalPosition);
        bool HasSpace();
        CharacterColor GetColor();
    }
}