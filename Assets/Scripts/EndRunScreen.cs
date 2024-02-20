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
    [SerializeField] private GameObject normalTextGO;
    [SerializeField] private GameObject normalTitleGO;
    [SerializeField] private GameObject tutTextGO;
    [SerializeField] private GameObject tutTitleGO;
    
    public override void Init()
    {
        base.Init();
    }
    
    public override void Show()
    {
        base.Show();
        if (UserManager.Instance.Data.Level == 1 && GameManager.Instance.CurrentRunDamage < 0.1f)
        {
            tutTextGO.SetActive(true);
            tutTitleGO.SetActive(true);
            normalTextGO.SetActive(false);
            normalTitleGO.SetActive(false);
            rewardText.text = "0";
        }
        else
        {
            tutTextGO.SetActive(false);
            tutTitleGO.SetActive(false);
            normalTextGO.SetActive(true);
            normalTitleGO.SetActive(true);
            UserManager.Instance.SeenHitGiantTutorial();
            int c = LevelManager.Instance.GetPreviousRunReward();
            rewardText.text = "+" + c.ToString();
            UserManager.Instance.AddCoins(c);
        }
        monsterHealthUI.SetHealth(UserManager.Instance.Data.MonsterHealth);
        levelText.text = "Boss " + UserManager.Instance.Data.Level.ToString();
        coinText.text = UserManager.Instance.Data.Coins.ToString();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void OnClick_Continue()
    {
        if (UserManager.Instance.Data.Level == 1)
        {
            if (!TutorialManager.Instance.HasSeenLimb2())
            {
                GameManager.Instance.GoToPlayMode();
            }
            else
            {
                GameManager.Instance.GoToUpgradeMode();
            }
        }
        else
        {
            GameManager.Instance.GoToUpgradeMode();
        }
    }
}
