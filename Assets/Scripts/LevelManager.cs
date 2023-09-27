using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public int debugLevel = 0;

    [SerializeField] private LevelsConfig Config;

#if UNITY_EDITOR
    public int CurrentLevelIndex => debugLevel > 0 ? debugLevel : UserManager.Instance.Data.Level;
#else
    public int CurrentLevelIndex => UserManager.Instance.Data.Level;
#endif

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
        
        GameManager.Instance.Monster = Instantiate(CurrentLevelData.MonsterPrefab.gameObject, GameManager.Instance.Map.GetMonsterParent()).GetComponentInChildren<Monster>();
        GameManager.Instance.Monster.transform.localPosition = Vector3.zero;
        GameManager.Instance.Monster.transform.localRotation = Quaternion.identity;
        GameManager.Instance.Monster.Init(CurrentLevelData.MonsterData);

    }
}
