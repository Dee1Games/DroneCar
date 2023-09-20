using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private LevelsConfig Config;

    public int CurrentLevelIndex => UserManager.Instance.Data.Level;
    public LevelData CurrentLevelData => Config.Levels[(CurrentLevelIndex - 1)%Config.Levels.Count];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void InitCurrentLevel()
    {
        if (GameManager.Instance.Map != null)
        {
            DestroyImmediate(GameManager.Instance.Map.gameObject);
        }
        if (GameManager.Instance.Monster != null)
        {
            DestroyImmediate(GameManager.Instance.Monster.gameObject);
        }

        GameManager.Instance.Map = Instantiate(CurrentLevelData.MapPrefab.gameObject).GetComponent<Map>();
        GameManager.Instance.Map.transform.position = Vector3.zero;
        GameManager.Instance.Map.transform.rotation = Quaternion.identity;
        
        GameManager.Instance.Monster = Instantiate(CurrentLevelData.MonsterPrefab.gameObject, GameManager.Instance.Map.GetMonsterParent()).GetComponent<Monster>();
        GameManager.Instance.Monster.transform.localPosition = Vector3.zero;
        GameManager.Instance.Monster.transform.localRotation = Quaternion.identity;
        GameManager.Instance.Monster.Init();
    }

    public float GetCurrentMonsterHealth()
    {
        return Config.InitMonsterHealth + (CurrentLevelIndex * Config.AddUpMonsterHealth);
    }

    public int GetRunReward()
    {
        return Mathf.RoundToInt(MergePlatform.Instance.GetCurrentUpgradePrice() + (UserManager.Instance.Data.Run * Config.RewardRunMultiplier) + (GameManager.Instance.CurrentRunDamage * Config.RewardDamageMultiplier));
    }

    public float GetSpaceLimit()
    {
        return Config.SpaceLimit;
    }
}
