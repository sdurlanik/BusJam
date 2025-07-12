using Sdurlanik.BusJam.Models;
using System.Collections.Generic;
using Sdurlanik.BusJam.Core.Grid;
using Sdurlanik.BusJam.MVC.Models;
using UnityEngine;

namespace Sdurlanik.BusJam.LevelEditor
{
    public class LevelEditor : MonoBehaviour
    {
        [Header("Configurations")] 
        public LevelSO LevelToEdit;
        public PrefabConfiguration PrefabConfig;
        public GridConfiguration GridConfig;

        [Header("Scene References")]
        public Transform LevelHolder;

        private readonly List<GameObject> _spawnedObjects = new List<GameObject>();
        private readonly List<Material> _instancedMaterials = new List<Material>();

        public void DisplayLevel()
        {
            ClearLevel();

            if (LevelToEdit == null || PrefabConfig == null || LevelHolder == null) return;

            DrawMainGrid();
            DrawWaitingArea();
            PlaceBuses();
            PlaceObstacles();
            PlaceCharacters();
        }

        public void ClearLevel()
        {
            foreach (var obj in _spawnedObjects)
            {
                if (obj != null) DestroyImmediate(obj);
            }
            _spawnedObjects.Clear();
        
            foreach (var material in _instancedMaterials)
            {
                if (material != null) DestroyImmediate(material, true);
            }
            _instancedMaterials.Clear();
        }
        
        private void DrawMainGrid()
        {
            if (GridConfig.MainGridTilePrefab == null) return;

            for (int x = 0; x < LevelToEdit.MainGridSize.x; x++)
            {
                for (int y = 0; y < LevelToEdit.MainGridSize.y; y++)
                {
                    var tile = Instantiate(GridConfig.MainGridTilePrefab, new Vector3(x, 0, y), Quaternion.identity, LevelHolder);
                    _spawnedObjects.Add(tile);
                }
            }
        }
        
        private void DrawWaitingArea()
        {
            if (GridConfig.WaitingAreaTilePrefab == null) return;
            
            var waitingAreaOrigin = new Vector3(0, 0, LevelToEdit.MainGridSize.y + GridConfig.SpacingBetweenGrids);

            for (int x = 0; x < GridConfig.WaitingGridSize.x; x++)
            {
                for (int y = 0; y < GridConfig.WaitingGridSize.y; y++)
                {
                    var position = waitingAreaOrigin + new Vector3(x, 0, y);
                    var tile = Instantiate(GridConfig.WaitingAreaTilePrefab, position, Quaternion.identity, LevelHolder);
                    _spawnedObjects.Add(tile);
                }
            }
        }

        private void PlaceBuses()
        {
            if (PrefabConfig.BusPrefab == null || LevelToEdit.BusColorSequence == null || LevelToEdit.BusColorSequence.Count == 0) return;
            
            var currentBusColor = LevelToEdit.BusColorSequence[0];
            var currentBusInstance = Instantiate(PrefabConfig.BusPrefab, LevelToEdit.BusStopPosition, Quaternion.identity, LevelHolder);
            SetObjectColor(currentBusInstance, currentBusColor);
            _spawnedObjects.Add(currentBusInstance);

            if (LevelToEdit.BusColorSequence.Count > 1)
            {
                var nextBusColor = LevelToEdit.BusColorSequence[1];
                var nextBusPosition = LevelToEdit.BusStopPosition + LevelToEdit.NextBusOffset;
                var nextBusInstance = Instantiate(PrefabConfig.BusPrefab, nextBusPosition, Quaternion.identity, LevelHolder);
                SetObjectColor(nextBusInstance, nextBusColor);
                _spawnedObjects.Add(nextBusInstance);
            }
        }
        
        private void SetObjectColor(GameObject instance, CharacterColor color)
        {
            var renderer = instance.GetComponentInChildren<MeshRenderer>();
            if (renderer == null) return;
            
            var newMaterial = new Material(renderer.sharedMaterial) { color = ColorMapper.GetColorFromEnum(color) };
            renderer.material = newMaterial;
            _instancedMaterials.Add(newMaterial);
        }

        private void PlaceObstacles()
        {
            if (PrefabConfig.ObstaclePrefab == null || LevelToEdit.Obstacles == null) return;

            foreach (var obstacleData in LevelToEdit.Obstacles)
            {
                var position = new Vector3(obstacleData.GridPosition.x, 0.5f, obstacleData.GridPosition.y);
                var obstacle = Instantiate(PrefabConfig.ObstaclePrefab, position, Quaternion.identity, LevelHolder);
                _spawnedObjects.Add(obstacle);
            }
        }

        private void PlaceCharacters()
        {
            if (PrefabConfig.CharacterPrefab == null || LevelToEdit.Characters == null) return;
            
            foreach (var charData in LevelToEdit.Characters)
            {
                var position = new Vector3(charData.GridPosition.x, 0.5f, charData.GridPosition.y);
                var character = Instantiate(PrefabConfig.CharacterPrefab, position, Quaternion.identity, LevelHolder);
                SetObjectColor(character, charData.Color);
                _spawnedObjects.Add(character);
            }
        }
        
        public Vector2Int WorldToGridPosition(Vector3 worldPosition)
        {
            int x = Mathf.RoundToInt(worldPosition.x);
            int y = Mathf.RoundToInt(worldPosition.z);
    
            x = Mathf.Clamp(x, 0, LevelToEdit.MainGridSize.x - 1);
            y = Mathf.Clamp(y, 0, LevelToEdit.MainGridSize.y - 1);

            return new Vector2Int(x, y);
        }
    }
}