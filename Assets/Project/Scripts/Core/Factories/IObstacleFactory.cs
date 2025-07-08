using UnityEngine;

namespace Sdurlanik.BusJam.Core.Factories
{
    public interface IObstacleFactory
    {
        GameObject Create(Vector3 position);
    }
}