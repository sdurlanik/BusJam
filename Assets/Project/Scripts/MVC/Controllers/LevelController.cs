using Sdurlanik.BusJam.Core.Grid;
using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.Core.Factories;
using Sdurlanik.BusJam.Models;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.MVC.Controllers
{
    public class LevelController : ILevelController
    {
        private readonly SignalBus _signalBus;
        private readonly IGridSystemManager _gridSystemManager;
        private readonly ICharacterFactory _characterFactory;
        private readonly IObstacleFactory _obstacleFactory;
        
        public LevelController(SignalBus signalBus, IGridSystemManager gridSystemManager, ICharacterFactory characterFactory, IObstacleFactory obstacleFactory)
        {
            _signalBus = signalBus;
            _gridSystemManager = gridSystemManager;
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
            _gridSystemManager.CreateGrids(levelData);
            
            Debug.Log($"Loading Level: {levelData.name}");
    
            var mainGrid = _gridSystemManager.MainGrid;

            foreach (var obstacleData in levelData.Obstacles)
            {
                var obstaclePosition = mainGrid.GetWorldPosition(obstacleData.GridPosition);
                var obstacleInstance = _obstacleFactory.Create(obstaclePosition);
                mainGrid.PlaceObject(obstacleInstance, obstacleData.GridPosition);
            }
    
            foreach (var characterData in levelData.Characters)
            {
                var characterPosition = mainGrid.GetWorldPosition(characterData.GridPosition, 0.5f);
                var characterView = _characterFactory.Create(characterData.Color, characterPosition, characterData.GridPosition);
                mainGrid.PlaceObject(characterView.gameObject, characterData.GridPosition);
            }

            Debug.Log("Level Loaded Successfully. Firing LevelReadySignal.");
            _signalBus.Fire(new LevelReadySignal(levelData));
        }
    }
}