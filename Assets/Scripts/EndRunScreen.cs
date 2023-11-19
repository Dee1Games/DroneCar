using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndRunScreen : UIScreen
{
    public override UIScreenID ID => UIScreenID.EndRun;

    [SerializeField] private HealthUI monsterHealthUI;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private TMP_Text coinText;
    
    public override void Init()
    {
        base.Init();
    }
    
    public override void Show()
    {
        base.Show();
        monsterHealthUI.SetHealth(UserManager.Instance.Data.MonsterHealth);
        levelText.text = "Boss " + UserManager.Instance.Data.Level.ToString();
        coinText.text = UserManager.Instance.Data.Coins.ToString();
        rewardText.text = "+" + LevelManager.Instance.GetPreviousRunReward().ToString();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void OnClick_Continue()
    {
        GameManager.Instance.GoToUpgradeMode();
    }
}
