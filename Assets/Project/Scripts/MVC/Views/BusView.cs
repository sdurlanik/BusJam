using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace Sdurlanik.BusJam.MVC.Views
{
    public class BusView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MeshRenderer _busBodyRenderer;
        [SerializeField] private List<Transform> _passengerSlots;

        public void SetColor(Color color)
        {
            _busBodyRenderer.material.color = color;
        }

        public Transform GetSlotTransform(int index)
        {
            if (index >= 0 && index < _passengerSlots.Count)
            {
                return _passengerSlots[index];
            }
            return null;
        }

        public async UniTask AnimateArrival(Vector3 targetPosition)
        {
            transform.position = targetPosition + Vector3.left * 20f;
            await transform.DOMove(targetPosition, 1f).SetEase(Ease.OutCubic).ToUniTask();
        }

        public async UniTask AnimateDeparture()
        {
            await transform.DOMove(transform.position + Vector3.right * 20f, 1f).SetEase(Ease.InCubic).ToUniTask();
            Destroy(gameObject);
        }
    }
}