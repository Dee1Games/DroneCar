using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameScreen : UIScreen
{
    public override UIScreenID ID => UIScreenID.InGame;

    [SerializeField] private HealthUI monsterHealthUI;

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

    private void UpdateMonsterHealthBar(float currentHealth, float maxHealth)
    {
        monsterHealthUI.UpdateHealthUI(currentHealth, maxHealth); 
    }
}
