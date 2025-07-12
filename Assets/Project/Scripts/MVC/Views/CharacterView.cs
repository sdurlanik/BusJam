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
        public CharacterColor Color { get; private set; }
        public Vector2Int GridPosition { get; private set; }

        [Header("References")] 
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private CharacterSettingsSO _characterSettings;
        
        private SignalBus _signalBus;
        private IMovementTracker _movementTracker;
        private MaterialPropertyBlock _materialPropertyBlock;
        private static readonly int ColorProperty = Shader.PropertyToID("_BaseColor");

        private void Awake()
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
        }

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
            if(IsMoving) return;
            _signalBus.Fire(new CharacterClickedSignal(this));
        }

        private void SetVisualColor(CharacterColor color)
        {
            _materialPropertyBlock.SetColor(ColorProperty, ColorMapper.GetColorFromEnum(color));
            _meshRenderer.SetPropertyBlock(_materialPropertyBlock);
        }

        public async UniTask MoveAlongPath(List<Vector2Int> path)
        {
            _movementTracker.RegisterMovement();
            try
            {
                var sequence = DOTween.Sequence();
                float totalDuration = _characterSettings.MoveDuration;
                float durationPerSegment = path.Count > 1 ? totalDuration / (path.Count - 1) : totalDuration;

                for (var i = 1; i < path.Count; i++)
                {
                    var gridPos = path[i];
                    var worldPosition = new Vector3(gridPos.x, transform.position.y, gridPos.y);
                    sequence.Append(transform.DOMove(worldPosition, durationPerSegment).SetEase(Ease.Linear));
                }
                await sequence.Play().ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
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
                    .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
            }
            finally
            {
                _movementTracker.UnregisterMovement();
            }
        }
    }
}