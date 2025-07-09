using UnityEngine;

namespace Sdurlanik.BusJam.MVC.Models
{
    [CreateAssetMenu(fileName = "PrefabConfiguration", menuName = "Bus Jam/Prefab Configuration", order = 2)]
    public class PrefabConfiguration : ScriptableObject
    {
        [Header("Character Prefabs")] 
        public GameObject CharacterPrefab;
        public GameObject ObstaclePrefab;

        [Header("Bus Prefabs")] public GameObject BusPrefab;
    }
}