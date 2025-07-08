using Sdurlanik.BusJam.Models;
using UnityEngine;

namespace Sdurlanik.BusJam.Core.Factories
{
    public interface ICharacterFactory
    {
        GameObject Create(CharacterColor color, Vector3 position);
    }
}