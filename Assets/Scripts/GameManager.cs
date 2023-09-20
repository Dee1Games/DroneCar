using System;
using System.Collections;
using System.Collections.Generic;
using SupersonicWisdomSDK;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public Map Map;
    [HideInInspector] public Monster Monster;
    [HideInInspector] public PlayerVehicle Player;
    [HideInInspector] public float CurrentRunDamage;

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
        GoToUpgradeMode();
    }

    public void GoToPlayMode()
    {
        isPlaying = true;
        spawnPlayer();
        UIManager.Instance.ShowScreen(UIScreenID.InGame);
        MergePlatform.Instance.Hide();
        CurrentRunDamage = 0f;
        Monster.OnRunStarted();
    }

    public void GoToUpgradeMode()
    {
        LevelManager.Instance.InitCurrentLevel();
        isPlaying = false;
        spawnPlayer();
        MergePlatform.Instance.Init();
        MergePlatform.Instance.Show();
        UIManager.Instance.ShowScreen(UIScreenID.Merge);
        Debug.Log($"Run {UserManager.Instance.Data.Run} Started");
        try
        {
            SupersonicWisdom.Api.NotifyLevelStarted(UserManager.Instance.Data.Run, null);
        }
        catch
        {
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
        Player = Instantiate(VehiclesConfig.GetVehicle(UserManager.Instance.Data.CurrentVehicleID).Prefab).GetComponent<PlayerVehicle>();
        Transform spawnPoint = Map.GetRandomSpawnPoint();
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
