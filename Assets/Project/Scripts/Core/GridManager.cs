using System.Collections.Generic;
using UnityEngine;

namespace Sdurlanik.BusJam.Core
{
    public class GridManager : IGridManager
    {
        private GameObject[,] _grid;

        public void InitializeGrid(int width, int height)
        {
            _grid = new GameObject[width, height];
        }
        
        public void PlaceObject(GameObject obj, Vector2Int gridPosition)
        {
            if (IsPositionValid(gridPosition))
            {
                _grid[gridPosition.x, gridPosition.y] = obj;
            }
            else
            {
                Debug.LogError($"[GridManager] Invalid position to place object: {gridPosition}");
            }
        }

        public bool IsCellAvailable(Vector2Int gridPosition)
        {
            return IsPositionValid(gridPosition) && _grid[gridPosition.x, gridPosition.y] == null;
        }

        private bool IsPositionValid(Vector2Int gridPosition)
        {
            return gridPosition.x >= 0 && gridPosition.x < _grid.GetLength(0) &&
                   gridPosition.y >= 0 && gridPosition.y < _grid.GetLength(1);
        }

        public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
        {
            Debug.Log("Pathfinding logic will be implemented here.");
            return new List<Vector2Int>();
        }
    }
}