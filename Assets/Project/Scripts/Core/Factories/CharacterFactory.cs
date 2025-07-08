using Sdurlanik.BusJam.Models;
using UnityEngine;
using Zenject;

namespace Sdurlanik.BusJam.Core.Factories
{
    public class CharacterFactory : ICharacterFactory
    {
        private readonly DiContainer _container;
        private readonly GameObject _characterPrefab;

        public CharacterFactory(DiContainer container, GameObject characterPrefab)
        {
            _container = container;
            _characterPrefab = characterPrefab;
        }

        public GameObject Create(CharacterColor color, Vector3 position)
        {
            var characterInstance = _container.InstantiatePrefab(_characterPrefab, position, Quaternion.identity, null);
            
            // TODO: Set character color or other properties
            
            return characterInstance;
        }
    }
}