using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MergeScreen : UIScreen
{
    public override UIScreenID ID => UIScreenID.Merge;

    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text levelText;
    public override void Init()
    {
        base.Init();
    }

    public override void Show()
    {
        base.Show();
        RefreshUpgradePrice();
        coinsText.text = "$" + Prefs.Coins.ToString();
        levelText.text = "Level " + Prefs.Level.ToString();
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
        MergePlatform.Instance.SpawnUpgrade();
    }
}
