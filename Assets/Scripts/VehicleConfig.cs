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

    [SerializeField] private List<UpgradeConfig> upgrades;
    [SerializeField] private List<ItemsEntry> items;

    public UpgradeConfig GetUpgradeConfig(UpgradeType type)
    {
        return upgrades.FirstOrDefault(up => up.Type == type);
    }
    
    public float GetProbability(UpgradeType type)
    {
        return items.FirstOrDefault(i => i.Type == type).Probability;
    }
    
    public List<Item> GetItems(UpgradeType type)
    {
        return items.FirstOrDefault(i => i.Type == type).Items;
    }
    
    public Item GetUpgrade(UpgradeType type, int level)
    {
        return GetItems(type)[level-1];
    }
    
    [System.Serializable]
    class ItemsEntry
    {
        public UpgradeType Type;
        public float Probability;
        public List<Item> Items;
    }
}
