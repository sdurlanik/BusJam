using System.Collections.Generic;
using Sdurlanik.BusJam.Models;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelProgression", menuName = "Bus Jam/Level Progression", order = 3)]
public class LevelProgressionSO : ScriptableObject
{
    public List<LevelSO> Levels;
}