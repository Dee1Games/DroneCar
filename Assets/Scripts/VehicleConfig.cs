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

    [SerializeField] private List<UpgradeConfig> upgrades;
    [SerializeField] private List<ItemsEntry> items;

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
    
    [System.Serializable]
    class ItemsEntry
    {
        public UpgradeType Type;
        public int Probability;
        public List<Item> Items;
    }
}
