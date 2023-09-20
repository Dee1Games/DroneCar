using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsConfig", menuName = "ScriptableObjects/LevelsConfig")]
public class LevelsConfig : ScriptableObject
{
    public List<LevelData> Levels;
    public float RewardRunMultiplier;
    public float RewardDamageMultiplier;
    public float SpaceLimit = 35f;
    public float InitMonsterHealth = 2000f;
    public float AddUpMonsterHealth = 250f;
}
