using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public int debugLevel = 0;

    [SerializeField] private LevelsConfig Config;

#if UNITY_EDITOR
    public int CurrentLevelIndex => debugLevel > 0 ? debugLevel : UserManager.Instance.Data.Level;
#else
    public int CurrentLevelIndex => debugLevel > 0 ? debugLevel : UserManager.Instance.Data.Level;
    //public int CurrentLevelIndex => UserManager.Instance.Data.Level;
#endif
    
    public LevelData PreviousLevelData
    {
        get
        {
            if (CurrentLevelIndex <= 1)
            {
                return null;
            }
            else
            {
                return GetLevelDataByIndex(CurrentLevelIndex-1);
            }
        }
    }

    public LevelData CurrentLevelData
    {
        get
        {
            return GetLevelDataByIndex(CurrentLevelIndex);
        }
    }
    
    public LevelData NextLevelData
    {
        get
        {
            return GetLevelDataByIndex(CurrentLevelIndex + 1);
        }
    }

    public LevelData GetLevelDataByIndex(int index)
    {
        if (index-1 < Config.Levels.Count)
        {
            return Config.Levels[(index - 1) % Config.Levels.Count];
        }
        else
        {
            int levelsCount = Config.Levels.Count - 1;
            int x = ((index-2) % levelsCount)+1;
            return Config.Levels[x];
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void InitCurrentLevel()
    {
        if (GameManager.Instance.Monster != null)
        {
            List<GameObject> objs = GameManager.Instance.Monster.detachedLimbs;
            while (objs.Count > 0)
            {
                GameObject obj = objs[0];
                DestroyImmediate(obj);
                objs.RemoveAt(0);
            }
            DestroyImmediate(GameManager.Instance.Monster.gameObject);
            
            List<LimbSmoke> objs2 = FindObjectsOfType<LimbSmoke>().ToList();
            while (objs2.Count > 0)
            {
                GameObject obj = objs2[0].gameObject;
                DestroyImmediate(obj);
                objs2.RemoveAt(0);
            }
        }
        if (GameManager.Instance.Map != null)
        {
            DestroyImmediate(GameManager.Instance.Map.gameObject);
        }
        
        GameManager.Instance.Map = Instantiate(CurrentLevelData.MapPrefab.gameObject).GetComponent<Map>();
        GameManager.Instance.Map.transform.position = Vector3.zero;
        GameManager.Instance.Map.transform.rotation = Quaternion.identity;
        //GameManager.Instance.Map.Init();
        
        GameManager.Instance.Monster = Instantiate(CurrentLevelData.MonsterPrefab.gameObject, GameManager.Instance.Map.GetMonsterParent()).GetComponentInChildren<Monster>();
        GameManager.Instance.GiantCore = GameManager.Instance.Monster.GetComponentInChildren<Giant_Core>();
        GameManager.Instance.Monster.transform.localPosition = Vector3.zero;
        GameManager.Instance.Monster.transform.localRotation = Quaternion.identity;
        GameManager.Instance.Monster.Init(CurrentLevelData.MonsterData);
        
        WeakPoint.CurrentIndex = 0;
    }

    // public int GetRunReward()
    // {
    //     return MergePlatform.Instance.GetTotalUpgradePrice(5);
    //
    //     //return Mathf.RoundToInt(MergePlatform.Instance.GetCurrentUpgradePrice() + (UserManager.Instance.Data.Run * Config.RewardRunMultiplier) + (GameManager.Instance.CurrentRunDamage * Config.RewardDamageMultiplier));
    // }
    
    public int GetPreviousRunReward()
    {
        return MergePlatform.Instance.GetTotalUpgradePrice(5);
        //return Mathf.RoundToInt(MergePlatform.Instance.GetCurrentUpgradePrice() + ((UserManager.Instance.Data.Run-1) * Config.RewardRunMultiplier) + (GameManager.Instance.CurrentRunDamage * Config.RewardDamageMultiplier));
    }
    
    public int GetRunReward()
    {
        int lvl1 = GameManager.Instance.Player.GetUpgradeLevel(UpgradeType.Tire);
        int lvl2 = GameManager.Instance.Player.GetUpgradeLevel(UpgradeType.MaxSpeed);
        int lvl3 = GameManager.Instance.Player.GetUpgradeLevel(UpgradeType.Bonus);
        int avg = Mathf.FloorToInt((lvl1 + lvl2 + lvl3) / 3);

        int prc = GameManager.Instance.Player.Config.GetPrice(avg);

        return prc * 3;
        
        int runDamage = Mathf.CeilToInt(LevelManager.Instance.Config.AvrageMonsterHealth / GameManager.Instance.life);
        float multiplier = LevelManager.Instance.Config.GetPrizeMultiplier(UserManager.Instance.Data.Run);
        return Mathf.CeilToInt(runDamage * multiplier);
    }

    public float GetSpaceLimit()
    {
        //return Config.SpaceLimit;
        return 100f;
    }
}
