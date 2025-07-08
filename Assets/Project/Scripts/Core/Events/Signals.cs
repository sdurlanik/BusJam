using Sdurlanik.BusJam.Models;
using Sdurlanik.BusJam.MVC.Views;

namespace Sdurlanik.BusJam.Core.Events
{
    public struct LevelReadySignal { }
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
}