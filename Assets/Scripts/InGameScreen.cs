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

    private void OnEnable()
    {
        Monster.OnHealthChange += UpdateMonsterHealthBar;
    }

    private void OnDisable()
    {
        Monster.OnHealthChange -= UpdateMonsterHealthBar;
    }

    public override void Init()
    {
        base.Init();
    }
    
    public override void Show()
    {
        base.Show();
        coinsText.text = "$" + UserManager.Instance.Data.Coins.ToString();
        levelText.text = "Level " + UserManager.Instance.Data.Level.ToString();
    }
    
    public override void Hide()
    {
        base.Hide();
    }
    
    public void OnClick_Back()
    {
        GameManager.Instance.GoToUpgradeMode();
    }

    private void UpdateMonsterHealthBar(float currentHealth, float maxHealth)
    {
        monsterHealthUI.UpdateHealthUI(currentHealth, maxHealth); 
    }
}
