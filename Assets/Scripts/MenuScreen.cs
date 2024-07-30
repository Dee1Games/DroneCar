using System;
using System.Collections;
using System.Collections.Generic;
//using HomaGames.HomaBelly; //TODO HOMA
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

        //Analytics.MainMenuLoaded(); //TODO HOMA
        
        coinsText.text = UserManager.Instance.Data.Coins.ToString(); 
        levelText.text = "Boss " + UserManager.Instance.Data.Level.ToString();
        monsterHealthUI.SetHealth(UserManager.Instance.Data.MonsterHealth);

        foreach (UpgradeButtonUI btn in upgrades)
        {
            int max = GameManager.Instance.Player.Config.GetUpgradeConfig(btn.type).maxLevel-1;
            int lvl = GameManager.Instance.Player.GetUpgradeLevel(btn.type);
            float diff = 0f;
            if (lvl < max)
            {
                if (btn.type == UpgradeType.Bomb)
                {
                    float newVal = Mathf.Abs(GameManager.Instance.Player.Config.GetUpgradeConfig(btn.type).GetBomb(lvl + 1));
                    float oldVal = Mathf.Abs(GameManager.Instance.Player.Config.GetUpgradeConfig(btn.type).GetBomb(0));
                    diff = (newVal / oldVal) * 100f;
                }
                else if (btn.type == UpgradeType.Gun)
                {
                    float newVal = Mathf.Abs(GameManager.Instance.Player.Config.GetUpgradeConfig(btn.type).GetGun(lvl + 1));
                    float oldVal = Mathf.Abs(GameManager.Instance.Player.Config.GetUpgradeConfig(btn.type).GetGun(0));
                    diff = (newVal / oldVal) * 100f;
                }
                else if (btn.type == UpgradeType.Tire)
                {
                    float newVal = Mathf.Abs(GameManager.Instance.Player.Config.GetUpgradeConfig(btn.type).GetHandling(lvl + 1));
                    float oldVal = Mathf.Abs(GameManager.Instance.Player.Config.GetUpgradeConfig(btn.type).GetHandling(0));
                    diff = (newVal / oldVal) * 100f;
                }
                else if (btn.type == UpgradeType.Turbo)
                {
                    float newVal = Mathf.Abs(GameManager.Instance.Player.Config.GetUpgradeConfig(btn.type).GetMaxSpeed(lvl + 1));
                    float oldVal = Mathf.Abs(GameManager.Instance.Player.Config.GetUpgradeConfig(btn.type).GetMaxSpeed(0));
                    diff = (newVal / oldVal) * 100f;
                }
                else if (btn.type == UpgradeType.Bonus)
                {
                    float newVal = Mathf.Abs(GameManager.Instance.Player.Config.GetUpgradeConfig(btn.type).GetBonus(lvl + 1));
                    float oldVal = Mathf.Abs(GameManager.Instance.Player.Config.GetUpgradeConfig(btn.type).GetBonus(0));
                    diff = (newVal / oldVal) * 100f;
                }
                else if (btn.type == UpgradeType.MaxSpeed)
                {
                    float newVal = Mathf.Abs(GameManager.Instance.Player.Config.GetUpgradeConfig(btn.type).GetMaxSpeedMultiplier(lvl + 1));
                    float oldVal = Mathf.Abs(GameManager.Instance.Player.Config.GetUpgradeConfig(btn.type).GetMaxSpeedMultiplier(0));
                    diff = (newVal / oldVal) * 100f;
                }

                diff -= 100;
            }
            
            //int price = MergePlatform.Instance.GetCurrentUpgradePrice();
            int price = GameManager.Instance.Player.Config.GetPrice(lvl+1);
            bool isActive = UserManager.Instance.Data.Coins >= price;
            btn.Init(price, diff.ToString("F0"), isActive);

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
