using System.Collections.Generic;
using UnityEngine;

namespace Sdurlanik.BusJam.Core
{
    public class GridManager : IGridManager
    {
        private class PathNode
        {
            public Vector2Int position;
            public int gCost;
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
        
        private GameObject[,] _grid;
        private int _width;
        private int _height;
        
        public void InitializeGrid(int width, int height)
        {
            _width = width;
            _height = height;
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

        public void ClearCell(Vector2Int gridPosition)
        {
            if (IsPositionValid(gridPosition))
            {
                _grid[gridPosition.x, gridPosition.y] = null;
            }
        }
        
        public List<Vector2Int> FindPath(Vector2Int startPosition, Vector2Int endPosition)
        {
            var startNode = new PathNode(startPosition);

            var openList = new List<PathNode> { startNode };
            var closedList = new HashSet<Vector2Int>();

            startNode.gCost = 0;
            startNode.hCost = CalculateDistance(startPosition, endPosition);
            startNode.CalculateFCost();

            while (openList.Count > 0)
            {
                var currentNode = GetLowestFCostNode(openList);
                if (currentNode.position == endPosition)
                {
                    return CalculatePath(currentNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode.position);

                foreach (var neighbourNode in GetNeighbourList(currentNode))
                {
                    if (closedList.Contains(neighbourNode.position)) continue;

                    if (!IsCellAvailable(neighbourNode.position) && neighbourNode.position != endPosition)
                    {
                        closedList.Add(neighbourNode.position);
                        continue;
                    }

                    var tentativeGCost = currentNode.gCost + CalculateDistance(currentNode.position, neighbourNode.position);
                    if (tentativeGCost < neighbourNode.gCost || !openList.Contains(neighbourNode))
                    {
                        neighbourNode.parent = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistance(neighbourNode.position, endPosition);
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
            return xDistance + yDistance;
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

        private List<PathNode> GetNeighbourList(PathNode currentNode)
        {
            var neighbourList = new List<PathNode>();
            var pos = currentNode.position;

            if (pos.x - 1 >= 0) neighbourList.Add(new PathNode(new Vector2Int(pos.x - 1, pos.y))); // Left
            if (pos.x + 1 < _width) neighbourList.Add(new PathNode(new Vector2Int(pos.x + 1, pos.y))); // Right
            if (pos.y - 1 >= 0) neighbourList.Add(new PathNode(new Vector2Int(pos.x, pos.y - 1))); // Down
            if (pos.y + 1 < _height) neighbourList.Add(new PathNode(new Vector2Int(pos.x, pos.y + 1))); // Up

            return neighbourList;
        }
        
        private bool IsPositionValid(Vector2Int gridPosition)
        {
            return gridPosition.x >= 0 && gridPosition.x < _width &&
                   gridPosition.y >= 0 && gridPosition.y < _height;
        }
    }
}