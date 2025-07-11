using UnityEngine;
using System.Collections.Generic;

namespace Sdurlanik.BusJam.Models
{

    public enum CharacterColor
    {
        Red,
        Green,
        Blue,
        Yellow,
        Purple
    }

    [System.Serializable]
    public struct CharacterPlacementData
    {
        public CharacterColor Color;
        public Vector2Int GridPosition;
    }

    [System.Serializable]
    public struct ObstaclePlacementData
    {
        public Vector2Int GridPosition;
    }

    [CreateAssetMenu(fileName = "so_level_", menuName = "Bus Jam/Create Level SO", order = 0)]
    public class LevelSO : ScriptableObject
    {
        [Header("Level Info")]
        public int LevelNumber = 1;
        
        [Header("Grid Settings")]
        public Vector2Int MainGridSize = new Vector2Int(5, 5);

        [Header("Character Placements")]
        public List<CharacterPlacementData> Characters;

        [Header("Obstacle Placements")]
        public List<ObstaclePlacementData> Obstacles;

        [Header("Bus Settings")]
        public List<CharacterColor> BusColorSequence;
        public Vector3 BusStopPosition = new Vector3(2f, 0.5f, 8f);
        public Vector3 NextBusOffset = new Vector3(-4f, 0f, 0f);
        
        [Header("Level Constraints")]
        public float TimeLimitInSeconds = 120f;
    }
}