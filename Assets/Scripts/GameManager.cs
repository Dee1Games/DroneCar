using System;
using System.Collections;
using System.Collections.Generic;
//using HomaGames.HomaBelly; //TODO HOMA
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    #region Statics
    public const string CPlayer = "Player";
    #endregion

    public int retryPrice;
    public int life;
    public int coinReward;
    [HideInInspector] public bool Skip;
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

    private void Update()
    {
        if (!Skip && Input.GetMouseButtonDown(0))
        {
            Skip = true;
        }
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
        GoToUpgradeMode();
    }

    public void GoToPlayMode()
    {
        UserManager.Instance.ResetInGameVehicleUpgrades();
        CurrentRunDamage = 0f;
        isPlaying = true;
        spawnPlayer();
        /*Analytics.LevelStarted(UserManager.Instance.Data.Run);
        if (UserManager.Instance.Data.Attempt == 1)
        {
            Analytics.MissionStarted(LevelManager.Instance.CurrentLevelIndex.ToString());
        }*/ //TODO HOMA
        UIManager.Instance.ShowScreen(UIScreenID.InGame);
        MergePlatform.Instance.Hide();
        //Map.Init();
    }

    public void GoToUpgradeMode()
    {
        UserManager.Instance.ResetInGameVehicleUpgrades();
        isPlaying = false;
        spawnPlayer();
        UIManager.Instance.ShowScreen(UIScreenID.MainMenu);
        MergePlatform.Instance.Hide();

        
        if(GameManager.Instance.Monster.Health <= 0f)
        {
            LevelManager.Instance.InitCurrentLevel();
        }
        
        Map.Init();


        return;
        
        MergePlatform.Instance.Init();
        MergePlatform.Instance.Show();
        UIManager.Instance.ShowScreen(UIScreenID.Merge);
        MergePlatform.Instance.ShowTutorialsIfNeeded();
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
        
        Player.InitPlayMode();
        
        // if(isPlaying)
        //     Player.InitPlayMode();
        // else
        //     Player.InitShowCaseMode();

    }

    public bool IsPlaying()
    {
        return isPlaying;
    }
}
