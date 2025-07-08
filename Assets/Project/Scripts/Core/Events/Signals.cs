using Sdurlanik.BusJam.Models;

namespace Sdurlanik.BusJam.Events
{
    public struct LevelReadySignal { }
    public struct LevelSuccessSignal { }
    public struct LevelFailSignal { }
    
    public class LevelLoadRequestedSignal
    {
        public readonly LevelSO LevelData;
        public LevelLoadRequestedSignal(LevelSO levelData) => LevelData = levelData;
    }

    public class CharacterSelectedSignal
    {
        
    }
}