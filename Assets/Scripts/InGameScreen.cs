using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameScreen : UIScreen
{
    public override UIScreenID ID => UIScreenID.InGame;

    [SerializeField] private HealthUI monsterHealthUI;
    [SerializeField] private Image lifeTime;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject retryButton;
    [SerializeField] private Animator damageUI;
    [SerializeField] private TMP_Text life;

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

    private void Update()
    {
        if (GameManager.Instance.Player != null)
        {
            UpdateLifeTImeBar(GameManager.Instance.Player.CurrentLifTimeLeft);
        }
    }

    public override void Init()
    {
        base.Init();
    }
    
    public override void Show()
    {
        base.Show();
        life.text = UserManager.Instance.Data.Lifes.ToString() + " x";
        retryButton.SetActive(true);
        coinsText.text = UserManager.Instance.Data.Coins.ToString();
        // if (UserManager.Instance.Data.Level == 1)
        // {
        //     HideRetryButton();
        // }
        // else
        {
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
    
    public void OnClick_Monster()
    {
        if(GameManager.Instance.Player == null) 
            return;
        
        GameManager.Instance.Player.pointToMonster();
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
    
    private void UpdateLifeTImeBar(float val)
    {
        lifeTime.fillAmount = val;
    }

    private void HideRetryButton()
    {
        life.text = UserManager.Instance.Data.Lifes.ToString() + " x";
        retryButton.SetActive(false);
    }
    private void ShowDamageUI()
    {
        damageUI.SetTrigger("damage");
    }
}
