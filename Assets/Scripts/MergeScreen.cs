using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MergeScreen : UIScreen
{
    public override UIScreenID ID => UIScreenID.Merge;

    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Button playButton;
    public override void Init()
    {
        base.Init();
    }

    public override void Show()
    {
        base.Show();
        RefreshUpgradePrice();
        coinsText.text = UserManager.Instance.Data.Coins.ToString();
        levelText.text = "Boss " + UserManager.Instance.Data.Level.ToString();

        if (!UserManager.Instance.Data.SeenMergeTutorial)
        {
            if (UserManager.Instance.Data.UpgradeCount==1)
            {
                TutorialManager.Instance.ShowBuyHint();
            }

            playButton.interactable = false;
        }
        else
        {
            playButton.interactable = true;
        }
    }
    
    public override void Hide()
    {
        base.Hide();
    }

    private void RefreshUpgradePrice()
    {
        priceText.text = "$" + MergePlatform.Instance.GetCurrentUpgradePrice().ToString();
    }

    public void OnClick_Play()
    {
        GameManager.Instance.GoToPlayMode();
    }

    public void OnClick_Upgrade()
    {
        if (UserManager.Instance.Data.Coins >= MergePlatform.Instance.GetCurrentUpgradePrice())
        {
            MergePlatform.Instance.SpawnUpgrade();
            if (!UserManager.Instance.Data.SeenMergeTutorial && UserManager.Instance.Data.UpgradeCount==2)
            {
                TutorialManager.Instance.ShowBuyHint2();
            }
        }
    }
}
