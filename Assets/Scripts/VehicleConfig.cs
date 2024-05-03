using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "VehicleConfig", menuName = "ScriptableObjects/VehicleConfig")]
[System.Serializable]
public class VehicleConfig : ScriptableObject
{
    public float Acceleration = 1f;
    public float ReverseAcceleration = 1f;
    public float MaxSpeed = 1f;
    public float MaxSpeedMultiplier = 1f;
    public float Handeling = 1f;
    public float Bomb = 1f;
    public float Gun = 1f;
    public float JumpForce = 1f;
    public float FireRate = 0.5f;
    public bool AlwaysShoot = true;
    public float LifeTime;
    public float SpeedMultiplyer = 1f;
    public float GunShootZ = 100f;
    public float LimitZ = 100f;
    
    [SerializeField] private AnimationCurve priceCurve = new AnimationCurve();
    [SerializeField] private Vector2 priceRemap;

    [SerializeField] private List<UpgradeConfig> upgrades;
    [SerializeField] private List<ItemsEntry> items;

    public int GetPrice(int level)
    {
        float value = priceCurve.Evaluate(level);
        return Mathf.FloorToInt(Remap(value, priceCurve, priceRemap.x, priceRemap.y));
    }

    public UpgradeConfig GetUpgradeConfig(UpgradeType type)
    {
        return upgrades.FirstOrDefault(up => up.Type == type);
    }
    
    public int GetProbability(UpgradeType type)
    {
        return items.FirstOrDefault(i => i.Type == type).Probability;
    }
    
    public List<Item> GetItems(UpgradeType type)
    {
        return items.FirstOrDefault(i => i.Type == type).Items;
    }
    
    public Item GetItem(UpgradeType type, int level)
    {
        if (GetItems(type).Count <= level)
            return null;
        else
            return GetItems(type)[level];
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

    
    [System.Serializable]
    class ItemsEntry
    {
        public UpgradeType Type;
        public int Probability;
        public List<Item> Items;
    }
}
