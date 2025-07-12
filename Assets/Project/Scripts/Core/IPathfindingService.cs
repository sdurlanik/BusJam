using System.Collections.Generic;
using Sdurlanik.BusJam.Core.Grid;
using UnityEngine;

namespace Sdurlanik.BusJam.Core.Pathfinding
{
    public interface IPathfindingService
    {
        List<Vector2Int> FindPath(IGrid grid, Vector2Int startPosition, Vector2Int endPosition);
    }
}