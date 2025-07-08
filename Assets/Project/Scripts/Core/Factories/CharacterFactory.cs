using Sdurlanik.BusJam.Models;
using Sdurlanik.BusJam.MVC.Views;
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

        public CharacterView  Create(CharacterColor color, Vector3 position, Vector2Int gridPosition)
        {
            var characterInstance = _container.InstantiatePrefabForComponent<CharacterView>(_characterPrefab, position, Quaternion.identity, null);
            characterInstance.Initialize(color, gridPosition);
            
            return characterInstance;
        }
    }
}