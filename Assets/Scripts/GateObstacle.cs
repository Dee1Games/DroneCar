using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateObstacle : MonoBehaviour
{
    [SerializeField] private List<GateObstacleItem> items;
    [SerializeField] private float negativeChance = 0.25f;
    [SerializeField] private Obstacle obstacle;

    private Dictionary<GateObstacleType, GateObstacleItem> itemsDict;

    private GateObstacleType selected;
    private bool isUsed = false;
    private bool isNegative;
    private UpgradeType gateType;

    public void Set(GateObstacleType t)
    {
        selected = t;
    }

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

        gateType = UpgradeType.None;
        if (selected != GateObstacleType.Obstacle)
        {
            if (selected == GateObstacleType.BombGate)
            {
                gateType = UpgradeType.Bomb;
            } else if (selected == GateObstacleType.GunGate)
            {
                gateType = UpgradeType.Gun;
            } else if (selected == GateObstacleType.TireGate)
            {
                gateType = UpgradeType.Tire;
            }  else if (selected == GateObstacleType.TurboGate)
            {
                gateType = UpgradeType.Turbo;
            }
        }
        
        float rnd = UnityEngine.Random.Range(0f, 1f);
        if (rnd < negativeChance)
        {
            isNegative = true;
        }
        else
        {
            isNegative = false;
        }
        ActivateType(selected, 0, 0);
        if (gateType != UpgradeType.None)
        {
            Refresh();
        }
    }

    public void Refresh()
    {
        if (gateType == UpgradeType.None)
        {
            return;
        }
        
        int max = GameManager.Instance.Player.Config.GetUpgradeConfig(gateType).maxLevel;
        int current = GameManager.Instance.Player.GetUpgradeLevel(gateType);
        int next = current;
        int prev = current;

        if (current < max)
        {
            next = current + 1;
        }
        
        if (current > 1)
        {
            prev = current - 1;
        }

        int gateLevel = next;
        if (isNegative)
            gateLevel = prev;
        
        float diff = 0f;
        if (selected == GateObstacleType.BombGate)
        {
            diff = Mathf.Abs(GameManager.Instance.Player.Config.GetUpgradeConfig(gateType).GetBomb(gateLevel));
        } else if (selected == GateObstacleType.GunGate)
        {
            diff = Mathf.Abs(GameManager.Instance.Player.Config.GetUpgradeConfig(gateType).GetGun(gateLevel));
        } else if (selected == GateObstacleType.TireGate)
        {
            diff = Mathf.Abs(GameManager.Instance.Player.Config.GetUpgradeConfig(gateType).GetHandling(gateLevel));
        }  else if (selected == GateObstacleType.TurboGate)
        {
            diff = Mathf.Abs(GameManager.Instance.Player.Config.GetUpgradeConfig(gateType).GetMaxSpeed(gateLevel));
        }
        int intDiff = Mathf.CeilToInt(diff);
        ActivateType(selected, intDiff, gateLevel);
    }

    public void ActivateType(GateObstacleType selectedType, int diff, int level)
    {
        foreach (GateObstacleType type in itemsDict.Keys)
        {
            if (selectedType == type)
            {
                itemsDict[type].GO.SetActive(true);
                Gate gate = itemsDict[type].GO.GetComponent<Gate>();
                if (gate != null)
                {
                    gate.Init(isNegative, diff, level);
                }
            }
            else
            {
                itemsDict[type].GO.SetActive(false);
            }

            if (type == GateObstacleType.Obstacle)
            {
                obstacle.Init();
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
            if (selected == GateObstacleType.Obstacle)
            {
                GameManager.Instance.Player.Core.Die();
                obstacle.Explode();
            }
            else
            {
                int l = GameManager.Instance.Player.GetUpgradeLevel(gateType);
                int newL = isNegative ? (l - 1) : (l + 1);

                if (GameManager.Instance.Player.SetUpgrade(gateType, newL))
                {
                    GameManager.Instance.Player.ShowUpgradeVisuals();
                    if (!isNegative)
                    {
                        GameManager.Instance.Player.PlayUpgradeParticle();
                        GameManager.Instance.Player.PlayUpgradeAnim(gateType);
                    }
                    else
                    {
                        GameManager.Instance.Player.PlayDowngradeParticle();
                    }
                }
            }
            GameManager.Instance.Map.Refresh();
            DeactivateAll();
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
