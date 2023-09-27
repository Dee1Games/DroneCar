using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsConfig", menuName = "ScriptableObjects/LevelsConfig")]
public class LevelsConfig : ScriptableObject
{
    public int level;
    
    public List<LevelData> Levels;
}
