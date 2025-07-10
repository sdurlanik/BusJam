using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.Core.Grid
{
    public class Grid : IGrid
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
        
        private readonly int _width;
        private readonly int _height;
        private readonly Vector3 _origin;
        private readonly GameObject[,] _gridObjects;
        private readonly DiContainer _container;
        
        public Grid(int width, int height, Vector3 origin, GameObject tilePrefab, DiContainer container)
        {
            _width = width;
            _height = height;
            _origin = origin;
            _gridObjects = new GameObject[width, height];
            _container = container; // Container'ı sakla

            // Grid zeminini oluştur
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (tilePrefab != null)
                    {
                        var worldPos = GetWorldPosition(new Vector2Int(x, y));
                        // Artık container'ı doğrudan kullanabiliriz.
                        _container.InstantiatePrefab(tilePrefab, worldPos, Quaternion.identity, null);
                    }
                }
            }
        }
        
        public Vector3 GetWorldPosition(Vector2Int gridPosition, float yOffset = 0)
        {
            return _origin + new Vector3(gridPosition.x, yOffset, gridPosition.y);
        }
        public GameObject GetObjectAt(Vector2Int gridPosition)
        {
            if (IsPositionValid(gridPosition))
            {
                return _gridObjects[gridPosition.x, gridPosition.y];
            }
            return null;
        }
        
         public void PlaceObject(GameObject obj, Vector2Int gridPosition)
        {
            if (IsPositionValid(gridPosition))
            {
                _gridObjects[gridPosition.x, gridPosition.y] = obj;
            }
            else
            {
                Debug.LogError($"[GridManager] Invalid position to place object: {gridPosition}");
            }
        }

        public bool IsCellAvailable(Vector2Int gridPosition)
        {
            return IsPositionValid(gridPosition) && _gridObjects[gridPosition.x, gridPosition.y] == null;
        }

        public void ClearCell(Vector2Int gridPosition)
        {
            if (IsPositionValid(gridPosition))
            {
                _gridObjects[gridPosition.x, gridPosition.y] = null;
            }
        }
        
        public List<Vector2Int> FindPath(Vector2Int startPosition, Vector2Int endPosition)
        {
            var openList = new List<PathNode>();
            var closedList = new HashSet<Vector2Int>();
            
            // Düğümleri pozisyonlarına göre hızlıca bulmak için bir dictionary.
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

                foreach (var neighbourPos in GetNeighbourPositions(currentNode.position))
                {
                    if (closedList.Contains(neighbourPos)) continue;

                    if (!IsCellAvailable(neighbourPos) && neighbourPos != endPosition)
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

        private List<Vector2Int> GetNeighbourPositions(Vector2Int currentPosition)
        {
            var neighbourList = new List<Vector2Int>();
            
            if (currentPosition.x - 1 >= 0) neighbourList.Add(new Vector2Int(currentPosition.x - 1, currentPosition.y));
            if (currentPosition.x + 1 < _width) neighbourList.Add(new Vector2Int(currentPosition.x + 1, currentPosition.y));
            if (currentPosition.y - 1 >= 0) neighbourList.Add(new Vector2Int(currentPosition.x, currentPosition.y - 1));
            if (currentPosition.y + 1 < _height) neighbourList.Add(new Vector2Int(currentPosition.x, currentPosition.y + 1));

            return neighbourList;
        }
        
        private bool IsPositionValid(Vector2Int gridPosition)
        {
            return gridPosition.x >= 0 && gridPosition.x < _width &&
                   gridPosition.y >= 0 && gridPosition.y < _height;
        }
        
        public int GetOccupiedCellCount()
        {
            int count = 0;
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (_gridObjects[x, y] != null)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        
        public void ClearAllCells()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (_gridObjects[x, y] != null)
                    {
                        GameObject.Destroy(_gridObjects[x, y]);
                        _gridObjects[x, y] = null;
                    }
                }
            }
        }
    }
}