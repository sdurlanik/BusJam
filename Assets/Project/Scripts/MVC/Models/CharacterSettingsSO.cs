using UnityEngine;

namespace Sdurlanik.BusJam.Project.Scripts.MVC.Models
{
    [CreateAssetMenu(fileName = "CharacterSettings", menuName = "Bus Jam/Settings/CharacterSettings", order = 1)]
    public class CharacterSettingsSO : ScriptableObject
    {
        public float MoveDuration = 0.2f;
    }
}