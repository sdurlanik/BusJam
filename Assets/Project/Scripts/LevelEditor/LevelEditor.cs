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

        private List<GameObject> _spawnedObjects = new List<GameObject>();
        private readonly List<Material> _instancedMaterials = new List<Material>();

        public void DisplayLevel()
        {
            ClearLevel();

            if (LevelToEdit == null || PrefabConfig == null || LevelHolder == null)
            {
                Debug.LogError("Please assign all required fields in the LevelEditor component!");
                return;
            }

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
            for (int x = 0; x < LevelToEdit.MainGridSize.x; x++)
            {
                for (int y = 0; y < LevelToEdit.MainGridSize.y; y++)
                {
                    if (GridConfig.MainGridTilePrefab != null)
                    {
                        var tile = Instantiate(GridConfig.MainGridTilePrefab, new Vector3(x, 0, y), Quaternion.identity,
                            LevelHolder);
                        _spawnedObjects.Add(tile);
                    }
                }
            }
        }
        
        private void DrawWaitingArea()
        {
            var waitingAreaOrigin = new Vector3(0, 0, LevelToEdit.MainGridSize.y + GridConfig.SpacingBetweenGrids);

            for (int x = 0; x < GridConfig.WaitingGridSize.x; x++)
            {
                for (int y = 0; y < GridConfig.WaitingGridSize.y; y++)
                {
                    if (GridConfig.WaitingAreaTilePrefab != null)
                    {
                        var position = waitingAreaOrigin + new Vector3(x, 0, y);
                        var tile = Instantiate(GridConfig.WaitingAreaTilePrefab, position, Quaternion.identity, LevelHolder);
                        _spawnedObjects.Add(tile);
                    }
                }
            }
        }

        private void PlaceBuses()
        {
            if (PrefabConfig.BusPrefab == null || LevelToEdit.BusColorSequence.Count == 0)
            {
                return;
            }

            var currentBusColor = LevelToEdit.BusColorSequence[0];
            var currentBusInstance = Instantiate(PrefabConfig.BusPrefab, LevelToEdit.BusStopPosition, Quaternion.identity, LevelHolder);
            SetBusColor(currentBusInstance, currentBusColor);
            _spawnedObjects.Add(currentBusInstance);

            if (LevelToEdit.BusColorSequence.Count > 1)
            {
                var nextBusColor = LevelToEdit.BusColorSequence[1];
                var nextBusPosition = LevelToEdit.BusStopPosition + LevelToEdit.NextBusOffset;
                var nextBusInstance = Instantiate(PrefabConfig.BusPrefab, nextBusPosition, Quaternion.identity, LevelHolder);
                SetBusColor(nextBusInstance, nextBusColor);
                _spawnedObjects.Add(nextBusInstance);
            }
        }
        
        private void SetBusColor(GameObject busInstance, CharacterColor color)
        {
            var renderer = busInstance.GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            {
                var newMaterial = new Material(renderer.sharedMaterial) { color = ColorMapper.GetColorFromEnum(color) };
                renderer.material = newMaterial;
                _instancedMaterials.Add(newMaterial);
            }
        }

        private void PlaceObstacles()
        {
            if (PrefabConfig.ObstaclePrefab == null) return;

            foreach (var obstacleData in LevelToEdit.Obstacles)
            {
                var position = new Vector3(obstacleData.GridPosition.x, 0.5f, obstacleData.GridPosition.y);
                var obstacle = Instantiate(PrefabConfig.ObstaclePrefab, position, Quaternion.identity, LevelHolder);
                _spawnedObjects.Add(obstacle);
            }
        }

        private void PlaceCharacters()
        {
            if (LevelToEdit.Characters == null)
            {
                Debug.LogError("No characters added to level");
                return;
            }
            
            foreach (var charData in LevelToEdit.Characters)
            {
                var position = new Vector3(charData.GridPosition.x, 0.5f, charData.GridPosition.y);
                var character = Instantiate(PrefabConfig.CharacterPrefab, position, Quaternion.identity, LevelHolder);

                var renderer = character.GetComponentInChildren<MeshRenderer>();
                if(renderer != null)
                {
                    var newMaterial = new Material(renderer.sharedMaterial) { color = GetColorFromEnum(charData.Color) };
                    renderer.material = newMaterial;
                    _instancedMaterials.Add(newMaterial);
                }

                _spawnedObjects.Add(character);
            }
        }

        private Color GetColorFromEnum(CharacterColor color)
        {
            switch (color)
            {
                case CharacterColor.Red: return Color.red;
                case CharacterColor.Green: return Color.green;
                case CharacterColor.Blue: return Color.blue;
                case CharacterColor.Yellow: return Color.yellow;
                case CharacterColor.Purple: return new Color(0.5f, 0, 0.5f);
                default: return Color.white;
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