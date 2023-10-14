using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance;

    [HideInInspector] public UserData Data;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void Init()
    {
        Data = SaveManager.Instance.LoadUserData();
        
        if (Data == null)
        {
            Data = new UserData();
            SaveManager.Instance.SaveUserData(Data);
        }
    }

    public void NextUpgrade()
    {
        Data.UpgradeCount++;
        SaveManager.Instance.SaveUserData(Data);
    }

    public void ResetVehicleUpgrades()
    {
        foreach (VehicleUpgradeData v in Data.VehicleUpgrades)
        {
            v.UpgradeLevels = new List<UpgradeLevel>()
            {
                new UpgradeLevel() {Type = UpgradeType.Tire, Level = 0},
                new UpgradeLevel() {Type = UpgradeType.Turbo, Level = 0},
                new UpgradeLevel() {Type = UpgradeType.Gun, Level = 0},
                new UpgradeLevel() {Type = UpgradeType.Bomb, Level = 0}
            };
        }
    }

    public void NextLevel()
    {
        ResetVehicleUpgrades();
        MergePlatform.Instance.ClearPlatform();
        Data.Level++;
        Data.MonsterHealth = 1f;
        SaveManager.Instance.SaveUserData(Data);
    }

    public void NextRun()
    {
        AddCoins(LevelManager.Instance.GetRunReward());
        Data.Run++;
        SaveManager.Instance.SaveUserData(Data);
    }
    
    public void AddCoins(int coins)
    {
        Data.Coins += LevelManager.Instance.GetRunReward();
        SaveManager.Instance.SaveUserData(Data);
    }
    
    public List<UpgradeLevel> GetUpgradeLevels(VehicleID id)
    {
        return Data.VehicleUpgrades.FirstOrDefault(v => v.VehicleID == id).UpgradeLevels;
    }

    public void SetUpgradeLevel(VehicleID id, UpgradeType type, int level)
    {
        foreach (VehicleUpgradeData upgradeData in Data.VehicleUpgrades)
        {
            if (upgradeData.VehicleID == id)
            {
                foreach (UpgradeLevel upgradeLevel in upgradeData.UpgradeLevels)
                {
                    if (upgradeLevel.Type == type)
                    {
                        upgradeLevel.Level = level;
                    }
                }
            }
        }
        SaveManager.Instance.SaveUserData(Data);
    }
    
    public int GetUpgradeLevel(VehicleID id, UpgradeType type)
    {
        foreach (VehicleUpgradeData upgradeData in Data.VehicleUpgrades)
        {
            if (upgradeData.VehicleID == id)
            {
                foreach (UpgradeLevel upgradeLevel in upgradeData.UpgradeLevels)
                {
                    if (upgradeLevel.Type == type)
                    {
                        return upgradeLevel.Level;
                    }
                }
            }
        }

        return 0;
    }

    public void SetMergePlatformCell(int index, UpgradeType type, int level)
    {
        Data.MergePlatform[index].Type = type;
        Data.MergePlatform[index].Level = level;
        SaveManager.Instance.SaveUserData(Data);
    }

    public UpgradeLevel GetMergePlatformCell(int index)
    {
        return Data.MergePlatform[index];
    }
    
    public void SetMonsterHealth(float health)
    {
        Data.MonsterHealth = health;
        SaveManager.Instance.SaveUserData(Data);
    }

    public void ResetMergePlatform()
    {
        Data.MergePlatform = new List<UpgradeLevel>()
        {
            new UpgradeLevel {Type = UpgradeType.Tire, Level = -1},
            new UpgradeLevel {Type = UpgradeType.Tire, Level = -1},
            new UpgradeLevel {Type = UpgradeType.Tire, Level = -1},
            new UpgradeLevel {Type = UpgradeType.Tire, Level = -1},
            new UpgradeLevel {Type = UpgradeType.Tire, Level = -1},
            new UpgradeLevel {Type = UpgradeType.Tire, Level = -1}
        };
        SaveManager.Instance.SaveUserData(Data);
    }
    
    public void SeenMergeTutorial()
    {
        Data.SeenMergeTutorial = true;
        SaveManager.Instance.SaveUserData(Data);
    }

    public void SeenMoveTutorial()
    {
        Data.SeenMoveTutorial = true;
        SaveManager.Instance.SaveUserData(Data);
    }

    public void SeenFlyTutorial()
    {
        Data.SeenFlyTutorial = true;
        SaveManager.Instance.SaveUserData(Data);
    }

    public void SeenHitGiantTutorial()
    {
        Data.SeenHitGiantTutorial = true;
        SaveManager.Instance.SaveUserData(Data);
    }
}
