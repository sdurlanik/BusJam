using UnityEngine;

[CreateAssetMenu(fileName = "GridConfiguration", menuName = "Bus Jam/Grid Configuration", order = 1)]
public class GridConfiguration : ScriptableObject
{
    [Header("Main Grid Settings")]
    public int MainGridWidth = 5;
    public int MainGridHeight = 5;
    public GameObject MainGridTilePrefab;

    [Header("Waiting Area Settings")]
    public int WaitingGridWidth = 5;
    public int WaitingGridHeight = 1;
    public GameObject WaitingAreaTilePrefab;
    
    [Header("Layout Settings")]
    public float SpacingBetweenGrids = 2f;
}