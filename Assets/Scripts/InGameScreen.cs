using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameScreen : UIScreen
{
    public override UIScreenID ID => UIScreenID.InGame;

    [SerializeField] private HealthUI monsterHealthUI;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject retryButton;

    private void OnEnable()
    {
        Monster.OnHealthChange += UpdateMonsterHealthBar;
        PlayerVehicle.OnExploded += HideRetryButton;
        
        _camera = Camera.main;
    }

    private void OnDisable()
    {
        Monster.OnHealthChange -= UpdateMonsterHealthBar;
        PlayerVehicle.OnExploded -= HideRetryButton;
    }

    public override void Init()
    {
        base.Init();
    }
    
    public override void Show()
    {
        base.Show();
        retryButton.SetActive(true);
        coinsText.text = UserManager.Instance.Data.Coins.ToString();
        levelText.text = "Boss " + UserManager.Instance.Data.Level.ToString();
        monsterHealthUI.SetHealth(UserManager.Instance.Data.MonsterHealth);
    }

    public override void Hide()
    {
        base.Hide();
    }
    
    public void OnClick_Back()
    {
        GameManager.Instance.GoToUpgradeMode();
    }

    private Camera _camera;
    
    public void OnClick_Retry()
    {
        GameManager.Instance.Player.Core.End(true, false);
        
        GameManager.Instance.GoToUpgradeMode();
    }

    private void UpdateMonsterHealthBar(float currentHealth, float maxHealth)
    {
        monsterHealthUI.UpdateHealthUI(currentHealth, maxHealth); 
    }

    private void HideRetryButton()
    {
        retryButton.SetActive(false);
    }
}
