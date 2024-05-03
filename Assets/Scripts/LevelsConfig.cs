using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsConfig", menuName = "ScriptableObjects/LevelsConfig")]
public class LevelsConfig : ScriptableObject
{
    public List<LevelData> Levels;
    
    public float AvrageMonsterHealth = 2000f;
    [SerializeField] private AnimationCurve prizeMultiplierCurve = new AnimationCurve();
    [SerializeField] private Vector2 prizeMultiplierRemap;

    public float GetPrizeMultiplier(int run)
    {
        float value = prizeMultiplierCurve.Evaluate(run);
        return Remap(value, prizeMultiplierCurve, prizeMultiplierRemap.x, prizeMultiplierRemap.y);
    }
    
    public static float Remap(float x, AnimationCurve curve, float newMin, float newMax)
    {
        float min = 9999f;
        float max = -9999f;
        for (int i = 0; i < curve.length; i++)
        {
            if (curve[i].value < min)
                min = curve[i].value;
            if (curve[i].value > max)
                max = curve[i].value;
        }

        if (max == min || curve.length==0)
        {
            return x;
        }
        else
        {
            float remappedValue = newMin + (x - min) / (max - min) * (newMax - newMin);
            return remappedValue;
        }
    }
}
