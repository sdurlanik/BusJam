using Sdurlanik.BusJam.Models;
using Sdurlanik.BusJam.MVC.Controllers;
using Sdurlanik.BusJam.MVC.Views;

namespace Sdurlanik.BusJam.Core.Events
{
    public struct LevelReadySignal
    {
        public readonly Models.LevelSO LevelData;
        public LevelReadySignal(Models.LevelSO levelData) => LevelData = levelData;
    }
    public struct LevelSuccessSignal { }
    public struct LevelFailSignal { }
    
    public class LevelLoadRequestedSignal
    {
        public readonly LevelSO LevelData;
        public LevelLoadRequestedSignal(LevelSO levelData) => LevelData = levelData;
    }

    public struct CharacterClickedSignal
    {
        public readonly CharacterView ClickedCharacter;
        public CharacterClickedSignal(CharacterView clickedCharacter) => ClickedCharacter = clickedCharacter;
    }
    
    public struct BusArrivedSignal
    {
        public readonly IBusController ArrivedBus;
        public BusArrivedSignal(MVC.Controllers.IBusController bus) => ArrivedBus = bus;
    }

    public struct BusFullSignal
    {
        public readonly IBusController FullBus;
        public BusFullSignal(MVC.Controllers.IBusController bus) => FullBus = bus;
    }
}