using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndLevelScreen : UIScreen
{
    public override UIScreenID ID => UIScreenID.EndLevel;

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
        levelText.text = "Boss " + (UserManager.Instance.Data.Level-1).ToString();
        coinText.text = UserManager.Instance.Data.Coins.ToString();
        rewardText.text = "+" + LevelManager.Instance.GetRunReward().ToString();
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
