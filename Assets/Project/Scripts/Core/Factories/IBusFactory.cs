using System.Threading;
using Cysharp.Threading.Tasks;
using Sdurlanik.BusJam.Models;
using Sdurlanik.BusJam.MVC.Controllers;
using UnityEngine;

namespace Sdurlanik.BusJam.Core.Factories
{
    public interface IBusFactory
    {
        UniTask<IBusController> Create(CharacterColor color, Vector3 arrivalPosition, CancellationToken cancellationToken);
        IBusController CreateAtPosition(CharacterColor color, Vector3 position);
    }
}