using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateObstacle : MonoBehaviour
{
    [SerializeField] private List<GateObstacleItem> items;
    [SerializeField] private float negativeChance = 0.25f;

    private Dictionary<GateObstacleType, GateObstacleItem> itemsDict;

    private GateObstacleType selected;
    private bool isUsed = false;
    private bool isNegative;

    public void Init()
    {
        isUsed = false;
        itemsDict = new Dictionary<GateObstacleType, GateObstacleItem>();
        foreach (GateObstacleItem item in items)
        {
            if (!itemsDict.ContainsKey(item.Type))
            {
                itemsDict.Add(item.Type, item);
            }
        }

        float rnd = UnityEngine.Random.Range(0f, 1f);
        float i = 0f;
        selected = GateObstacleType.Obstacle;
        foreach (GateObstacleType type in itemsDict.Keys)
        {
            float chance = itemsDict[type].Chance;
            if (rnd >= i && rnd < i+chance)
            {
                selected = type;
            }
            i += chance;
        }
        
        rnd = UnityEngine.Random.Range(0f, 1f);
        if (rnd < negativeChance)
        {
            isNegative = true;
        }
        else
        {
            isNegative = false;
        }
        ActivateType(selected);
    }

    public void ActivateType(GateObstacleType selectedType)
    {
        foreach (GateObstacleType type in itemsDict.Keys)
        {
            if (selectedType == type)
            {
                itemsDict[type].GO.SetActive(true);
                Gate gate = itemsDict[type].GO.GetComponent<Gate>();
                if (gate != null)
                {
                    gate.Init(isNegative);
                }
            }
            else
            {
                itemsDict[type].GO.SetActive(false);
            }
        }
    }
    
    public void DeactivateAll()
    {
        foreach (GateObstacleType type in itemsDict.Keys)
        {
            itemsDict[type].GO.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isUsed)
            return;
        
        if (other.GetComponentInParent<PlayerVehicle>() != null)
        {
            isUsed = true;
            DeactivateAll();
            if (selected == GateObstacleType.Obstacle)
            {
                
            }
            else
            {
                UpgradeType upgradeType = UpgradeType.None;
                if (selected == GateObstacleType.BombGate)
                {
                    upgradeType = UpgradeType.Bomb;
                } else if (selected == GateObstacleType.GunGate)
                {
                    upgradeType = UpgradeType.Gun;
                } else if (selected == GateObstacleType.TireGate)
                {
                    upgradeType = UpgradeType.Tire;
                }  else if (selected == GateObstacleType.TurboGate)
                {
                    upgradeType = UpgradeType.Turbo;
                }

                int l = GameManager.Instance.Player.GetUpgradeLevel(upgradeType);
                int newL = isNegative ? (l - 1) : (l + 1);

                if (GameManager.Instance.Player.SetUpgrade(upgradeType, newL))
                {
                    GameManager.Instance.Player.ShowUpgradeVisuals();
                    if (!isNegative)
                    {
                        GameManager.Instance.Player.PlayUpgradeParticle();
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class GateObstacleItem
    {
        public GateObstacleType Type;
        public float Chance;
        public GameObject GO;
    }

    public enum GateObstacleType
    {
        Obstacle,
        TurboGate,
        TireGate,
        GunGate,
        BombGate,
    }
}
