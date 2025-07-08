using Sdurlanik.BusJam.Models;
using Sdurlanik.BusJam.MVC.Views;
using UnityEngine;

namespace Sdurlanik.BusJam.Core.Factories
{
    public interface ICharacterFactory
    {
        CharacterView  Create(CharacterColor color, Vector3 position, Vector2Int gridPosition);
    }
}