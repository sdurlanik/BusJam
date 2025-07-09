using Sdurlanik.BusJam.Core.Factories;
using Sdurlanik.BusJam.MVC.Models;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.Installers
{
    public class FactoriesInstaller : MonoInstaller
    {
        [SerializeField] private PrefabConfiguration _prefabConfig;
        
        public override void InstallBindings()
        {
            Container.Bind<ICharacterFactory>().To<CharacterFactory>().AsSingle().WithArguments(_prefabConfig.CharacterPrefab);
            Container.Bind<IObstacleFactory>().To<ObstacleFactory>().AsSingle().WithArguments(_prefabConfig.ObstaclePrefab);
            Container.Bind<IBusFactory>().To<BusFactory>().AsSingle().WithArguments(_prefabConfig.BusPrefab);
        }
    }
}