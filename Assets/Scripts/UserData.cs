using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class UserData
{
    public int Level;
    public int Run;
    public VehicleID CurrentVehicleID;
    public List<VehicleID> OwnedVehicles;
    public List<VehicleUpgradeData> VehicleUpgrades;
    public int Coins;
    public int UpgradeCount;
    public List<UpgradeLevel> MergePlatform;

    public UserData()
    {
        Level = 1;
        Run = 1;
        CurrentVehicleID = VehicleID.Vehicle_01;
        OwnedVehicles = new List<VehicleID>() {VehicleID.Vehicle_01};
        VehicleUpgrades = new List<VehicleUpgradeData>()
        {
            new VehicleUpgradeData()
            {
                VehicleID = VehicleID.Vehicle_01,
                UpgradeLevels = new List<UpgradeLevel>()
                {
                    new UpgradeLevel() {Type = UpgradeType.Tire, Level = 0},
                    new UpgradeLevel() {Type = UpgradeType.Turbo, Level = 0},
                    new UpgradeLevel() {Type = UpgradeType.Gun, Level = 0},
                    new UpgradeLevel() {Type = UpgradeType.Bomb, Level = 0}
                }
            }
        };
        Coins = 9999999;
        UpgradeCount = 1;
        MergePlatform = new List<UpgradeLevel>()
        {
            new UpgradeLevel {Type = UpgradeType.Tire, Level = -1},
            new UpgradeLevel {Type = UpgradeType.Tire, Level = -1},
            new UpgradeLevel {Type = UpgradeType.Tire, Level = -1},
            new UpgradeLevel {Type = UpgradeType.Tire, Level = -1},
            new UpgradeLevel {Type = UpgradeType.Tire, Level = -1},
            new UpgradeLevel {Type = UpgradeType.Tire, Level = -1}
        };
    }
}
