using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsConfig", menuName = "ScriptableObjects/LevelsConfig")]
public class LevelsConfig : ScriptableObject
{
    public List<LevelData> Levels;
}
