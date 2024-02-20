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
    [SerializeField] private TMP_Text levelText_tut;
    [SerializeField] private GameObject retryButton;
    [SerializeField] private Animator damageUI;

    private void OnEnable()
    {
        Monster.OnHealthChange += UpdateMonsterHealthBar;
        PlayerVehicle.OnExploded += HideRetryButton;
        PlayerVehicle.OnTookDamage += ShowDamageUI;
        
        _camera = Camera.main;
    }

    private void OnDisable()
    {
        Monster.OnHealthChange -= UpdateMonsterHealthBar;
        PlayerVehicle.OnExploded -= HideRetryButton;
        PlayerVehicle.OnTookDamage -= ShowDamageUI;
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
        if (UserManager.Instance.Data.Level == 1)
        {
            levelText_tut.gameObject.SetActive(true);
            levelText.gameObject.SetActive(false);
            HideRetryButton();
        }
        else
        {
            levelText_tut.gameObject.SetActive(false);
            levelText.gameObject.SetActive(true);
            levelText.text = "Boss " + UserManager.Instance.Data.Level.ToString();
        }
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
    private void ShowDamageUI()
    {
        damageUI.SetTrigger("damage");
    }
}
