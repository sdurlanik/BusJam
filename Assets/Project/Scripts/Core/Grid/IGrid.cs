using System.Collections.Generic;
using UnityEngine;

namespace Sdurlanik.BusJam.Core.Grid
{
    public interface IGrid
    {
        int Width { get; }
        int Height { get; }
        void PlaceObject(GameObject obj, Vector2Int gridPosition);
        void ClearCell(Vector2Int gridPosition);
        bool IsCellAvailable(Vector2Int gridPosition);
        GameObject GetObjectAt(Vector2Int gridPosition);
        Vector3 GetWorldPosition(Vector2Int gridPosition, float yOffset = 0);
        int GetOccupiedCellCount();
        void ClearAllCells();
    }
}