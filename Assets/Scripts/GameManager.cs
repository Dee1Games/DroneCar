using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private Monster Monster;
    [SerializeField] private float respawnDelay;
    [SerializeField] private Transform[] spawnPoints;

    private void OnEnable()
    {
        PlayerVehicle.OnExploded += SpawnPlayer;
    }

    private void OnDisable()
    {
        PlayerVehicle.OnExploded -= SpawnPlayer;
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
        Prefs.Coins = 99999;
        UIManager.Instance.Init();
        GoToUpgradeMode();
    }

    public void GoToPlayMode()
    {
        PlayerVehicle.Instance.InitPlayMode();
        UIManager.Instance.ShowScreen(UIScreenID.InGame);
        MergePlatform.Instance.Hide();
        Monster.Init();
        spawnPlayer();
    }

    public void GoToUpgradeMode()
    {
        PlayerVehicle.Instance.InitShowCaseMode();
        MergePlatform.Instance.Init();
        MergePlatform.Instance.Show();
        UIManager.Instance.ShowScreen(UIScreenID.Merge);
    }

    public void SpawnPlayer()
    {
        Invoke("spawnPlayer", respawnDelay);
    }

    private void spawnPlayer()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        PlayerVehicle.Instance.transform.position = spawnPoint.position;
        PlayerVehicle.Instance.transform.forward = spawnPoint.forward;
        PlayerVehicle.Instance.InitPlayMode();
    }
}
