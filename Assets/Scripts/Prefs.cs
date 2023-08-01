using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefs
{
    private const string KEY_COINS = "Coins";
    private const string KEY_LEVEL = "Level";
    private const string KEY_UPGRADECOUNT = "UpgradeCount";

    public static int Coins
    {
        get
        {
            return PlayerPrefs.GetInt(KEY_COINS, 0);
        }
        set
        {
            if (value < 0)
                return;
            PlayerPrefs.SetInt(KEY_COINS, value);
        }
    }
    
    public static int Level
    {
        get
        {
            return PlayerPrefs.GetInt(KEY_LEVEL, 1);
        }
        set
        {
            PlayerPrefs.SetInt(KEY_LEVEL, value);
        }
    }
    
    public static int UpgradeCount
    {
        get
        {
            return PlayerPrefs.GetInt(KEY_UPGRADECOUNT, 0);
        }
        set
        {
            PlayerPrefs.SetInt(KEY_UPGRADECOUNT, value);
        }
    }
}
