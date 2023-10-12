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

    [SerializeField] private Image upgradeButton;
    [SerializeField] private Sprite activeButtonSprite;
    [SerializeField] private Sprite passiveButtonSprite;
    
    public override void Init()
    {
        base.Init();
    }

    public override void Show()
    {
        base.Show();
        RefreshUpgradePrice();
        coinsText.text = "$" + UserManager.Instance.Data.Coins.ToString();
        levelText.text = "Level " + UserManager.Instance.Data.Level.ToString();
    }
    
    public override void Hide()
    {
        base.Hide();
    }

    private void RefreshUpgradePrice()
    {
        int price = MergePlatform.Instance.GetCurrentUpgradePrice();
        priceText.text = "$" + price.ToString();

        if (price > UserManager.Instance.Data.Coins)
        {
            upgradeButton.sprite = passiveButtonSprite;
        }
        else
        {
            upgradeButton.sprite = activeButtonSprite;
        }
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
