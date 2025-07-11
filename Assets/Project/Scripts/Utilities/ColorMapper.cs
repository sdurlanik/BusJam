using Sdurlanik.BusJam.Models;
using UnityEngine;

public static class ColorMapper
{
    public static Color GetColorFromEnum(CharacterColor color)
    {
        switch (color)
        {
            case CharacterColor.Red: return Color.red;
            case CharacterColor.Green: return Color.green;
            case CharacterColor.Blue: return Color.blue;
            case CharacterColor.Yellow: return Color.yellow;
            case CharacterColor.Purple: return new Color(0.5f, 0, 0.5f);
            default: return Color.white;
        }
    }
}