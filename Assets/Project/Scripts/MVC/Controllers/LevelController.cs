using Sdurlanik.BusJam.Core;
using Sdurlanik.BusJam.Events;
using Sdurlanik.BusJam.Models;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.Controllers
{
    public class LevelController : ILevelController
    {
        private readonly SignalBus _signalBus;
        private readonly IGridManager _gridManager;

        public LevelController(SignalBus signalBus, IGridManager gridManager)
        {
            _signalBus = signalBus;
            _gridManager = gridManager;
            
            _signalBus.Subscribe<LevelLoadRequestedSignal>(OnLevelLoadRequested);
        }

        private void OnLevelLoadRequested(LevelLoadRequestedSignal signal)
        {
            LoadLevel(signal.LevelData);
        }

        public void LoadLevel(LevelSO levelData)
        {
            Debug.Log($"Loading Level: {levelData.name}");

            _gridManager.InitializeGrid(levelData.GridWidth, levelData.GridHeight);

            foreach (var obstacleData in levelData.Obstacles)
            {
                // TODO: use a factory or pool for obstacle instantiation
                
                var obstaclePrefab = Resources.Load<GameObject>("Prefabs/Obstacle");
                var obstacleInstance = Object.Instantiate(obstaclePrefab, new Vector3(obstacleData.GridPosition.x, 0, obstacleData.GridPosition.y), Quaternion.identity);
                _gridManager.PlaceObject(obstacleInstance, obstacleData.GridPosition);
            }
            
            foreach (var characterData in levelData.Characters)
            {
                // TODO: use a factory or pool for character instantiation
                var characterPrefab = Resources.Load<GameObject>("Prefabs/Character");
                var characterInstance = Object.Instantiate(characterPrefab, new Vector3(characterData.GridPosition.x, 0.5f, characterData.GridPosition.y), Quaternion.identity);
                
                _gridManager.PlaceObject(characterInstance, characterData.GridPosition);
            }

            Debug.Log("Level Loaded Successfully. Firing LevelReadySignal.");
            _signalBus.Fire<LevelReadySignal>();
        }
    }
}