using Sdurlanik.BusJam.Models;
using Sdurlanik.BusJam.MVC.Controllers;
using Sdurlanik.BusJam.MVC.Views;

namespace Sdurlanik.BusJam.Core.Events
{
    public struct LevelReadySignal
    {
        public readonly LevelSO LevelData;
        public LevelReadySignal(LevelSO levelData) => LevelData = levelData;
    }
    public struct LevelSuccessSignal { }
    public struct GameOverSignal { }
    
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
        public BusArrivedSignal(IBusController bus) => ArrivedBus = bus;
    }

    public struct BusFullSignal
    {
        public readonly IBusController FullBus;
        public BusFullSignal(IBusController bus) => FullBus = bus;
    }
}