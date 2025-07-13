using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.Core.Grid;
using Sdurlanik.BusJam.Models;
using UnityEngine;
using Zenject;
using System;

namespace Sdurlanik.BusJam.Core.Camera
{
    public class CameraController : IInitializable, IDisposable
    {
        private readonly float _fieldOfView = 60f;
        private readonly float _padding = 1.15f; // %15 padding 

        private readonly SignalBus _signalBus;
        private readonly UnityEngine.Camera _camera;
        private readonly GridConfiguration _gridConfig;

        public CameraController(SignalBus signalBus, UnityEngine.Camera camera, GridConfiguration gridConfig)
        {
            _signalBus = signalBus;
            _camera = camera;
            _gridConfig = gridConfig;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<LevelReadySignal>(OnLevelReady);
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<LevelReadySignal>(OnLevelReady);
        }

        private void OnLevelReady(LevelReadySignal signal)
        {
            AdjustCameraToFit(signal.LevelData);
        }

        private void AdjustCameraToFit(LevelSO levelData)
        {
            _camera.orthographic = false;
            _camera.fieldOfView = _fieldOfView;
            _camera.transform.rotation = Quaternion.Euler(60f, 0, 0);

            var gameplayBounds = CalculateGameplayBounds(levelData);

            var worldWidth = gameplayBounds.size.x;
            var worldHeight = gameplayBounds.size.z;
            
            var verticalFovRad = _fieldOfView * Mathf.Deg2Rad;
            var horizontalFovRad = 2f * Mathf.Atan(Mathf.Tan(verticalFovRad / 2f) * _camera.aspect);

            var distanceForHeight = (worldHeight / 2f) / Mathf.Tan(verticalFovRad / 2f);
            var distanceForWidth = (worldWidth / 2f) / Mathf.Tan(horizontalFovRad / 2f);
            
            var requiredDistance = Mathf.Max(distanceForWidth, distanceForHeight);
            
            var targetPoint = gameplayBounds.center;
            targetPoint.x = (levelData.MainGridSize.x - 1) / 2f;

            var backDirection = -_camera.transform.forward;
            var cameraPosition = targetPoint + backDirection * requiredDistance * _padding;
            
            _camera.transform.position = cameraPosition;
        }

        private Bounds CalculateGameplayBounds(LevelSO levelData)
        {
            var bounds = new Bounds();
            bounds.Encapsulate(Vector3.zero);
            bounds.Encapsulate(new Vector3(levelData.MainGridSize.x, 0, levelData.MainGridSize.y));

            var waitingAreaXOffset = (levelData.MainGridSize.x - _gridConfig.WaitingGridSize.x) / 2f;
            var waitingAreaZPos = levelData.MainGridSize.y + _gridConfig.SpacingBetweenGrids;
            
            var waitingAreaStart = new Vector3(waitingAreaXOffset, 0, waitingAreaZPos);
            var waitingAreaEnd = waitingAreaStart + new Vector3(_gridConfig.WaitingGridSize.x, 0, _gridConfig.WaitingGridSize.y);
            
            bounds.Encapsulate(waitingAreaStart);
            bounds.Encapsulate(waitingAreaEnd);
            
            bounds.Encapsulate(levelData.BusStopPosition);
            bounds.Encapsulate(levelData.BusStopPosition + levelData.NextBusOffset);

            return bounds;
        }
    }
}