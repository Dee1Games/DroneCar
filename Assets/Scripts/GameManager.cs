using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    #region Statics
    public const string CPlayer = "Player";
    #endregion

    [HideInInspector] public Map Map;
    [HideInInspector] public Monster Monster;
    [HideInInspector] public Giant_Core GiantCore;
    [HideInInspector] public PlayerVehicle Player;
    [HideInInspector] public float CurrentRunDamage;
    [HideInInspector] public RunResult RunResult;

    [SerializeField] private Vehicles VehiclesConfig;
    [SerializeField] private float respawnDelay;

    private bool isPlaying;

    private void OnEnable()
    {
        //PlayerVehicle.OnExploded += SpawnPlayer;
    }

    private void OnDisable()
    {
        //PlayerVehicle.OnExploded -= SpawnPlayer;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Init();
    }
    
    public void Init()
    {
        UserManager.Instance.Init();
        LevelManager.Instance.InitCurrentLevel();
        UIManager.Instance.Init();
        if (UserManager.Instance.Data.Level == 1)
        {
            GoToPlayMode();
        }
        else
        {
            GoToUpgradeMode();
        }
    }

    public void GoToPlayMode()
    {
        UserManager.Instance.ResetVehicleUpgrades();
        Map.Init();
        CurrentRunDamage = 0f;
        isPlaying = true;
        spawnPlayer();
        UIManager.Instance.ShowScreen(UIScreenID.InGame);
        MergePlatform.Instance.Hide();
    }

    public void GoToUpgradeMode()
    {
        UserManager.Instance.ResetVehicleUpgrades();
        isPlaying = false;
        spawnPlayer();
        MergePlatform.Instance.Init();
        MergePlatform.Instance.Show();
        UIManager.Instance.ShowScreen(UIScreenID.Merge);
        MergePlatform.Instance.ShowTutorialsIfNeeded();
        Debug.Log($"Run {UserManager.Instance.Data.Run} Started");
        try
        {
            //SupersonicWisdom.Api.NotifyLevelStarted(UserManager.Instance.Data.Run, null);
        }
        catch
        {
        }
        
        if(GameManager.Instance.Monster.Health <= 0f)
        {
            LevelManager.Instance.InitCurrentLevel();
        }
    }

    public void SpawnPlayer()
    {
        Invoke("spawnPlayer", respawnDelay);
    }

    private void spawnPlayer()
    {
        if(Player != null)
            DestroyImmediate(Player.gameObject);
        Player = Instantiate(VehiclesConfig.GetVehicle(LevelManager.Instance.CurrentLevelData.Vehicle).Prefab).GetComponent<PlayerVehicle>();
        Transform spawnPoint = Map.GetSpawnPoint();
        Player.transform.position = spawnPoint.position;
        Player.transform.forward = spawnPoint.forward;
        
        if(isPlaying)
            Player.InitPlayMode();
        else
            Player.InitShowCaseMode();

    }

    public bool IsPlaying()
    {
        return isPlaying;
    }
}
