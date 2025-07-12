using System.Collections.Generic;
using Sdurlanik.BusJam.Core.Grid;
using UnityEngine;

namespace Sdurlanik.BusJam.Core.Pathfinding
{
    public class PathfindingService : IPathfindingService
    {
        private class PathNode
        {
            public Vector2Int position;
            public int gCost = int.MaxValue;
            public int hCost;
            public int fCost;
            public PathNode parent;

            public PathNode(Vector2Int position)
            {
                this.position = position;
            }

            public void CalculateFCost()
            {
                fCost = gCost + hCost;
            }
        }

        public List<Vector2Int> FindPath(IGrid grid, Vector2Int startPosition, Vector2Int endPosition)
        {
            var openList = new List<PathNode>();
            var closedList = new HashSet<Vector2Int>();
            var pathNodeMap = new Dictionary<Vector2Int, PathNode>();

            var startNode = new PathNode(startPosition)
            {
                gCost = 0,
                hCost = CalculateDistance(startPosition, endPosition)
            };
            startNode.CalculateFCost();
            openList.Add(startNode);
            pathNodeMap[startPosition] = startNode;

            while (openList.Count > 0)
            {
                var currentNode = GetLowestFCostNode(openList);
                if (currentNode.position == endPosition)
                {
                    return CalculatePath(currentNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode.position);

                foreach (var neighbourPos in GetNeighbourPositions(grid, currentNode.position))
                {
                    if (closedList.Contains(neighbourPos)) continue;

                    if (!grid.IsCellAvailable(neighbourPos) && neighbourPos != endPosition)
                    {
                        closedList.Add(neighbourPos);
                        continue;
                    }

                    var tentativeGCost = currentNode.gCost + CalculateDistance(currentNode.position, neighbourPos);

                    if (!pathNodeMap.TryGetValue(neighbourPos, out var neighbourNode) || tentativeGCost < neighbourNode.gCost)
                    {
                        if (neighbourNode == null)
                        {
                            neighbourNode = new PathNode(neighbourPos);
                            pathNodeMap[neighbourPos] = neighbourNode;
                        }

                        neighbourNode.parent = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistance(neighbourPos, endPosition);
                        neighbourNode.CalculateFCost();

                        if (!openList.Contains(neighbourNode))
                        {
                            openList.Add(neighbourNode);
                        }
                    }
                }
            }
            return null; 
        }
        
        private List<Vector2Int> CalculatePath(PathNode endNode)
        {
            var path = new List<Vector2Int>();
            path.Add(endNode.position);
            var currentNode = endNode;
            while (currentNode.parent != null)
            {
                path.Add(currentNode.parent.position);
                currentNode = currentNode.parent;
            }
            path.Reverse();
            return path;
        }

        private int CalculateDistance(Vector2Int a, Vector2Int b)
        {
            var xDistance = Mathf.Abs(a.x - b.x);
            var yDistance = Mathf.Abs(a.y - b.y);
            return 10 * (xDistance + yDistance);
        }
        
        private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
        {
            var lowestFCostNode = pathNodeList[0];
            for (int i = 1; i < pathNodeList.Count; i++)
            {
                if (pathNodeList[i].fCost < lowestFCostNode.fCost)
                {
                    lowestFCostNode = pathNodeList[i];
                }
            }
            return lowestFCostNode;
        }

        private List<Vector2Int> GetNeighbourPositions(IGrid grid, Vector2Int currentPosition)
        {
            var neighbourList = new List<Vector2Int>();

            if (currentPosition.x - 1 >= 0) neighbourList.Add(new Vector2Int(currentPosition.x - 1, currentPosition.y));
            if (currentPosition.x + 1 < grid.Width) neighbourList.Add(new Vector2Int(currentPosition.x + 1, currentPosition.y));
            if (currentPosition.y - 1 >= 0) neighbourList.Add(new Vector2Int(currentPosition.x, currentPosition.y - 1));
            if (currentPosition.y + 1 < grid.Height) neighbourList.Add(new Vector2Int(currentPosition.x, currentPosition.y + 1));

            return neighbourList;
        }
    }
}