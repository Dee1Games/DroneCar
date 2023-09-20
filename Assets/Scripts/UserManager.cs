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
        if (Data == null || !Data.SeenMergeTutorial)
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

    public void NextLevel()
    {
        Data.Level++;
        Data.MonsterHealth = 1f;
        Data.CurrentVehicleID = (VehicleID) (((int) Data.CurrentVehicleID+1) % 3);
        ResetVehicleUpgrades();
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
    
    public List<UpgradeLevel> GetUpgradeLevels()
    {
        return Data.VehicleUpgrades;
    }

    public void SetUpgradeLevel(VehicleID id, UpgradeType type, int level)
    {
        foreach (UpgradeLevel upgradeLevel in Data.VehicleUpgrades)
        {
            if (upgradeLevel.Type == type)
            {
                upgradeLevel.Level = level;
            }
        }
        SaveManager.Instance.SaveUserData(Data);
    }
    
    public int GetUpgradeLevel(UpgradeType type)
    {
        foreach (UpgradeLevel upgradeLevel in Data.VehicleUpgrades)
        {
            if (upgradeLevel.Type == type)
            {
                return upgradeLevel.Level;
            }
        }

        return 0;
    }

    public void ResetVehicleUpgrades()
    {
        Data.VehicleUpgrades = new List<UpgradeLevel>()
        {
            new UpgradeLevel() {Type = UpgradeType.Tire, Level = 0},
            new UpgradeLevel() {Type = UpgradeType.Turbo, Level = 0},
            new UpgradeLevel() {Type = UpgradeType.Gun, Level = 0},
            new UpgradeLevel() {Type = UpgradeType.Bomb, Level = 0}
        };
        SaveManager.Instance.SaveUserData(Data);
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

    public void SetCurrentVehicleID(VehicleID id)
    {
        Data.CurrentVehicleID = id;
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
}
