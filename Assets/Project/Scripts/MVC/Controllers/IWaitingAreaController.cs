using Cysharp.Threading.Tasks;
using Sdurlanik.BusJam.MVC.Views;

namespace Sdurlanik.BusJam.MVC.Controllers
{
    public interface IWaitingAreaController
    {
        UniTask AddCharacterToArea(CharacterView character);
    }
}