using Sdurlanik.BusJam.Core;
using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.Core.Factories;
using Sdurlanik.BusJam.Models;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.Controllers
{
    public class LevelController : ILevelController
    {
        private readonly SignalBus _signalBus;
        private readonly IGridManager _gridManager;
        private readonly ICharacterFactory _characterFactory;
        private readonly IObstacleFactory _obstacleFactory;
        
        public LevelController(SignalBus signalBus, IGridManager gridManager, ICharacterFactory characterFactory, IObstacleFactory obstacleFactory)
        {
            _signalBus = signalBus;
            _gridManager = gridManager;
            _characterFactory = characterFactory;
            _obstacleFactory = obstacleFactory;
            
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
                var obstaclePosition = new Vector3(obstacleData.GridPosition.x, 0, obstacleData.GridPosition.y);
                var obstacleInstance = _obstacleFactory.Create(obstaclePosition);
                _gridManager.PlaceObject(obstacleInstance, obstacleData.GridPosition);
            }
            
            foreach (var characterData in levelData.Characters)
            {
                var characterPosition = new Vector3(characterData.GridPosition.x, 0.5f, characterData.GridPosition.y);
                var characterInstance = _characterFactory.Create(characterData.Color, characterPosition);
                _gridManager.PlaceObject(characterInstance, characterData.GridPosition);
            }

            Debug.Log("Level Loaded Successfully. Firing LevelReadySignal.");
            _signalBus.Fire<LevelReadySignal>();
        }
    }
}