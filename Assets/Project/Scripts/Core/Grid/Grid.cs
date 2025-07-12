using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.Core.Grid
{
    public class Grid : IGrid
    {
        public int Width => _width;
        public int Height => _height;

        private readonly int _width;
        private readonly int _height;
        private readonly Vector3 _origin;
        private readonly GameObject[,] _gridObjects;
        private readonly DiContainer _container;

        private readonly List<GameObject> _instantiatedTiles;

        public Grid(int width, int height, Vector3 origin, GameObject tilePrefab, DiContainer container)
        {
            _width = width;
            _height = height;
            _origin = origin;
            _gridObjects = new GameObject[width, height];
            _container = container;

            _instantiatedTiles = new List<GameObject>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (tilePrefab != null)
                    {
                        var worldPos = GetWorldPosition(new Vector2Int(x, y));
                        var tileInstance =
                            _container.InstantiatePrefab(tilePrefab, worldPos, Quaternion.identity, null);
                        _instantiatedTiles.Add(tileInstance);
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

        private bool IsPositionValid(Vector2Int gridPosition)
        {
            var result = gridPosition.x >= 0 && gridPosition.x < _width &&
                         gridPosition.y >= 0 && gridPosition.y < _height;

            if (!result)
            {
                Debug.LogWarning($"[GridManager] Position {gridPosition} is invalid. Width: {_width}, Height: {_height}");
            }
            
            return result;
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
                        Object.Destroy(_gridObjects[x, y]);
                        _gridObjects[x, y] = null;
                    }
                }
            }

            foreach (var tile in _instantiatedTiles)
            {
                Object.Destroy(tile);
            }

            _instantiatedTiles.Clear();
        }
    }
}