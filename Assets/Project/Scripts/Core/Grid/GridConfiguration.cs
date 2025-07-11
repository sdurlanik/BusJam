using UnityEngine;

namespace Sdurlanik.BusJam.Core.Grid
{
    
        [CreateAssetMenu(fileName = "GridConfiguration", menuName = "Bus Jam/Grid Configuration", order = 1)]

    public class GridConfiguration : ScriptableObject
    {
        [Header("Tile Prefabs")] 
        public GameObject MainGridTilePrefab;
        public GameObject WaitingAreaTilePrefab;
        
        [Header("Waiting Area Dimensions")]
        public Vector2Int WaitingGridSize = new Vector2Int(5, 1);

        [Header("Layout Settings")] 
        public float SpacingBetweenGrids = 2f;
    }
}