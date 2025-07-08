using UnityEngine;

namespace Sdurlanik.BusJam.Core
{
    public interface IGridManager
    {
        void InitializeGrid(int width, int height);

        void PlaceObject(GameObject obj, Vector2Int gridPosition);
        
        bool IsCellAvailable(Vector2Int gridPosition);
        
        void ClearCell(Vector2Int gridPosition);
        
        System.Collections.Generic.List<Vector2Int> FindPath(Vector2Int start, Vector2Int end);
    }
}