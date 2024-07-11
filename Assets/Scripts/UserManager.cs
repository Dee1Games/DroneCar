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
            Data.Lifes = GameManager.Instance.life;
            SaveManager.Instance.SaveUserData(Data);
        }
        else
        {
            SeenFlyTutorial();
            SeenMoveTutorial();
        }
    }

    public void NextUpgrade()
    {
        Data.UpgradeCount++;
        SaveManager.Instance.SaveUserData(Data);
    }
    
    public void LostLife()
    {
        Data.Lifes--;
        SaveManager.Instance.SaveUserData(Data);
    }
    
    public void ResetLife()
    {
        Data.Lifes = GameManager.Instance.life;
        SaveManager.Instance.SaveUserData(Data);
    }
    
    public void AddLife()
    {
        Data.Lifes++;
        SaveManager.Instance.SaveUserData(Data);
    }

    public void ResetInGameVehicleUpgrades()
    {
        int i = 0;
        foreach (VehicleUpgradeData v in Data.VehicleUpgrades)
        {
            v.UpgradeLevels = new List<UpgradeLevel>()
            {
                new UpgradeLevel() {Type = UpgradeType.Tire, Level = Data.VehicleUpgrades[i].UpgradeLevels.FirstOrDefault(x=>x.Type==UpgradeType.Tire).Level},
                new UpgradeLevel() {Type = UpgradeType.Bonus, Level = Data.VehicleUpgrades[i].UpgradeLevels.FirstOrDefault(x=>x.Type==UpgradeType.Bonus).Level},
                new UpgradeLevel() {Type = UpgradeType.MaxSpeed, Level = Data.VehicleUpgrades[i].UpgradeLevels.FirstOrDefault(x=>x.Type==UpgradeType.MaxSpeed).Level},
                new UpgradeLevel() {Type = UpgradeType.Turbo, Level = 0},
                new UpgradeLevel() {Type = UpgradeType.Gun, Level = 0},
                new UpgradeLevel() {Type = UpgradeType.Bomb, Level = 0}
            };
            i++;
        }
    }
    
    public void ResetAllVehicleUpgrades()
    {
        foreach (VehicleUpgradeData v in Data.VehicleUpgrades)
        {
            v.UpgradeLevels = new List<UpgradeLevel>()
            {
                new UpgradeLevel() {Type = UpgradeType.Tire, Level = 0},
                new UpgradeLevel() {Type = UpgradeType.MaxSpeed, Level = 0},
                new UpgradeLevel() {Type = UpgradeType.Bonus, Level = 0},
                new UpgradeLevel() {Type = UpgradeType.Turbo, Level = 0},
                new UpgradeLevel() {Type = UpgradeType.Gun, Level = 0},
                new UpgradeLevel() {Type = UpgradeType.Bomb, Level = 0}
            };
        }
    }

    public void NextLevel()
    {
        ResetInGameVehicleUpgrades();
        MergePlatform.Instance.ClearPlatform();
        UserManager.Instance.ResetLife();
        UserManager.Instance.ResetRun();
        Data.Level++;
        Data.MonsterHealth = 1f;
        SaveManager.Instance.SaveUserData(Data);
    }

    public void NextRun()
    {
        Data.Run++;
        Data.Attempt++;
        SaveManager.Instance.SaveUserData(Data);
    }
    
    public void ResetRun()
    {
        Data.Attempt = 1;
        //Data.Run = 1;
        SaveManager.Instance.SaveUserData(Data);
    }
    
    public void AddCoins(int coins)
    {
        Data.Coins += coins;
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
