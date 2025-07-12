using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Sdurlanik.BusJam.Project.Scripts.MVC.Models;

namespace Sdurlanik.BusJam.MVC.Views
{
    public class BusView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MeshRenderer _busBodyRenderer;
        [SerializeField] private List<Transform> _passengerSlots;
        [SerializeField] private BusSettingsSO _busSettings;

        private MaterialPropertyBlock _materialPropertyBlock;
        private static readonly int ColorProperty = Shader.PropertyToID("_BaseColor");

        private void Awake()
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
        }

        public void SetColor(Color color)
        {
            _materialPropertyBlock.SetColor(ColorProperty, color);
            _busBodyRenderer.SetPropertyBlock(_materialPropertyBlock);
        }

        public Transform GetSlotTransform(int index)
        {
            return (index >= 0 && index < _passengerSlots.Count) ? _passengerSlots[index] : null;
        }

        public async UniTask AnimateDeparture(CancellationToken cancellationToken)
        {
            await transform.DOMove(transform.position + Vector3.right * _busSettings.MoveOffset, _busSettings.MoveDuration)
                .SetEase(Ease.InCubic)
                .ToUniTask(cancellationToken: cancellationToken); 
        }
        
        public async UniTask AnimateToStopPosition(Vector3 targetPosition, CancellationToken cancellationToken)
        {
            await transform.DOMove(targetPosition, _busSettings.MoveDuration)
                .SetEase(Ease.OutBack)
                .ToUniTask(cancellationToken: cancellationToken);
        }

        public async UniTask AnimateArrival(Vector3 targetPosition, CancellationToken cancellationToken)
        {
            transform.position = targetPosition + Vector3.left * _busSettings.MoveOffset;
            await transform.DOMove(targetPosition, _busSettings.MoveDuration)
                .SetEase(Ease.OutCubic)
                .ToUniTask(cancellationToken: cancellationToken);
        }
    }
}