using Cysharp.Threading.Tasks;
using Sdurlanik.BusJam.Models;
using Sdurlanik.BusJam.MVC.Controllers;
using Sdurlanik.BusJam.MVC.Models;
using Sdurlanik.BusJam.MVC.Views;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.Core.Factories
{
    public class BusFactory : IBusFactory
    {
        private readonly DiContainer _container;
        private readonly GameObject _busPrefab;

        public BusFactory(DiContainer container, GameObject busPrefab)
        {
            _container = container;
            _busPrefab = busPrefab;
        }

        public async UniTask<IBusController> Create(CharacterColor color, Vector3 arrivalPosition)
        {
            var busView = _container.InstantiatePrefabForComponent<BusView>(_busPrefab);

            var busModel = new BusModel(color, 3); //TODO: capacity should be configurable

            var busController = _container.Instantiate<BusController>(new object[] { busModel, busView });

            await busController.Initialize(arrivalPosition);

            return busController;
        }
    }
}