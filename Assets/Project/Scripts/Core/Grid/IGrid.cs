using System.Collections.Generic;
using UnityEngine;

namespace Sdurlanik.BusJam.Core.Grid
{
    public interface IGrid
    {
        void PlaceObject(GameObject obj, Vector2Int gridPosition);
        void ClearCell(Vector2Int gridPosition);
        bool IsCellAvailable(Vector2Int gridPosition);
        GameObject GetObjectAt(Vector2Int gridPosition);
        Vector3 GetWorldPosition(Vector2Int gridPosition, float yOffset = 0);
        List<Vector2Int> FindPath(Vector2Int startPosition, Vector2Int endPosition);

    }
}