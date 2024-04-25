using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuScreen : UIScreen
{
    public override UIScreenID ID => UIScreenID.MainMenu;
    
    [SerializeField] private HealthUI monsterHealthUI;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private List<UpgradeButtonUI> upgrades;

    public override void Init()
    {
        base.Init();
    }
    
    public override void Show()
    {
        base.Show();
        coinsText.text = UserManager.Instance.Data.Coins.ToString(); 
        levelText.text = "Boss " + UserManager.Instance.Data.Level.ToString();
        monsterHealthUI.SetHealth(UserManager.Instance.Data.MonsterHealth);

        foreach (UpgradeButtonUI btn in upgrades)
        {
            int max = GameManager.Instance.Player.Config.GetUpgradeConfig(btn.type).maxLevel-1;
            int lvl = GameManager.Instance.Player.GetUpgradeLevel(btn.type);
            int intDiff = 0;
            if (lvl < max)
            {
                float diff = 0f;
                if (btn.type == UpgradeType.Bomb)
                {
                    diff = Mathf.Abs(GameManager.Instance.Player.Config.GetUpgradeConfig(btn.type).GetBomb(lvl + 1));
                }
                else if (btn.type == UpgradeType.Gun)
                {
                    diff = Mathf.Abs(GameManager.Instance.Player.Config.GetUpgradeConfig(btn.type).GetGun(lvl + 1));
                }
                else if (btn.type == UpgradeType.Tire)
                {
                    diff = Mathf.Abs(GameManager.Instance.Player.Config.GetUpgradeConfig(btn.type)
                        .GetHandling(lvl + 1));
                }
                else if (btn.type == UpgradeType.Turbo)
                {
                    diff = Mathf.Abs(GameManager.Instance.Player.Config.GetUpgradeConfig(btn.type)
                        .GetMaxSpeed(lvl + 1));
                }
                intDiff = Mathf.CeilToInt(diff);
            }
            
            int price = MergePlatform.Instance.GetCurrentUpgradePrice();
            bool isActive = UserManager.Instance.Data.Coins >= price;
            btn.Init(price, intDiff, isActive);

            if (lvl >= max)
            {
                btn.SetMax();
            }
        }
    }

    public override void Hide()
    {
        base.Hide();
    }
    
    public void OnClick_Play()
    {
        GameManager.Instance.GoToPlayMode();
    }
}
