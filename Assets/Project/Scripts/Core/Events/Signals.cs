using Sdurlanik.BusJam.Models;
using Sdurlanik.BusJam.MVC.Controllers;
using Sdurlanik.BusJam.MVC.Views;

namespace Sdurlanik.BusJam.Core.Events
{
    public struct StartGameRequestedSignal { }
    
    public struct LevelReadySignal
    {
        public readonly LevelSO LevelData;
        public LevelReadySignal(LevelSO levelData) => LevelData = levelData;
    }
    
    public struct LevelCompletedSignal { }
    
    public struct GameOverSignal { }
    
    public struct LevelLoadRequestedSignal
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
    
    public struct AllBusesDispatchedSignal { }
    
    public struct TimerUpdatedSignal
    {
        public readonly float RemainingTime;
        public TimerUpdatedSignal(float remainingTime) => RemainingTime = remainingTime;
    }
    
    public struct ResetGameplaySignal { }

    public struct NextLevelRequestedSignal { }
    
    public struct RestartLevelRequestedSignal{ }
    
    public struct WaitingAreaChangedSignal { }
    
    public struct LevelCompleteSequenceFinishedSignal { }
    
    public struct TimeIsUpSignal { }
    
    public struct BusArrivalSequenceStartedSignal 
    {
        public readonly IBusController ArrivingBus;
        public BusArrivalSequenceStartedSignal(IBusController bus) => ArrivingBus = bus;
    }

    public struct CharacterEnteredWaitingAreaSignal
    {
        public readonly CharacterView Character;
        public CharacterEnteredWaitingAreaSignal(CharacterView character) => Character = character;
    }
}