using UnityEngine;

namespace Sdurlanik.BusJam.Project.Scripts.MVC.Models
{
    [CreateAssetMenu(fileName = "CharacterSettings", menuName = "Bus Jam/Settings/BusSettings", order = 1)]
    public class BusSettingsSO : ScriptableObject
    {
        public float MoveDuration = 0.3f;
        public float MoveOffset = 20f;
        public int Capacity = 3;
    }
}