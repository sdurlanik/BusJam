using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sdurlanik.BusJam.Core.Events;
using Sdurlanik.BusJam.Core.Movement;
using Sdurlanik.BusJam.Models;
using Sdurlanik.BusJam.Project.Scripts.MVC.Models;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.MVC.Views
{
    [RequireComponent(typeof(Collider))]
    public class CharacterView : MonoBehaviour
    {
        public bool IsMoving { get; set; }

        [Header("References")] 
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private CharacterSettingsSO _characterSettings;
        private SignalBus _signalBus;
        private IMovementTracker _movementTracker;

        public CharacterColor Color { get; private set; }
        public Vector2Int GridPosition { get; private set; }

        [Inject]
        public void Construct(SignalBus signalBus, IMovementTracker movementTracker)
        {
            _signalBus = signalBus;
            _movementTracker = movementTracker;
        }

        public void Initialize(CharacterColor color, Vector2Int gridPosition)
        {
            Color = color;
            GridPosition = gridPosition;

            SetVisualColor(color);
        }

        private void OnMouseDown()
        {
            Debug.Log($"Character clicked at {GridPosition} with color {Color}");

            _signalBus.Fire(new CharacterClickedSignal(this));
        }

        private void SetVisualColor(CharacterColor color)
        {
            //TODO: will change to use a material pool or similar optimization in the future.
            var materialColor = UnityEngine.Color.white;
            switch (color)
            {
                case CharacterColor.Red:
                    materialColor = UnityEngine.Color.red;
                    break;
                case CharacterColor.Green:
                    materialColor = UnityEngine.Color.green;
                    break;
                case CharacterColor.Blue:
                    materialColor = UnityEngine.Color.blue;
                    break;
                case CharacterColor.Yellow:
                    materialColor = UnityEngine.Color.yellow;
                    break;
                case CharacterColor.Purple:
                    materialColor = new UnityEngine.Color(0.5f, 0, 0.5f);
                    break;
            }

            _meshRenderer.material.color = materialColor;
        }

        public async UniTask MoveAlongPath(List<Vector2Int> path)
        {
            _movementTracker.RegisterMovement();

            try
            {
                var sequence = DOTween.Sequence();
                for (var i = 0; i < path.Count; i++)
                {
                    var gridPos = path[i];
                    var worldPosition = new Vector3(gridPos.x, transform.position.y, gridPos.y);
                    sequence.Append(transform.DOMove(worldPosition, _characterSettings.MoveDuration / path.Count).SetEase(Ease.Linear));
                }

                await sequence.Play().ToUniTask();
                Debug.Log("Path completed");
            }
            finally
            {
                _movementTracker.UnregisterMovement();
            }
        }

        public void UpdateGridPosition(Vector2Int newPosition)
        {
            GridPosition = newPosition;
        }

        public async UniTask MoveToPoint(Vector3 worldPosition)
        {
            _movementTracker.RegisterMovement();
            try
            {
                await transform.DOMove(worldPosition, _characterSettings.MoveDuration)
                    .SetEase(Ease.OutQuad)
                    .ToUniTask();
                
                Debug.Log("Point Reached");
            }
            finally
            {
                _movementTracker.UnregisterMovement();
            }
        }
    }
}