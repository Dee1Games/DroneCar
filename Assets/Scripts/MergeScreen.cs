using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MergeScreen : UIScreen
{
    public override UIScreenID ID => UIScreenID.Merge;

    [SerializeField] private TMP_Text[] priceTexts;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text levelText_tut;

    [SerializeField] private Image[] upgradeButtons;
    [SerializeField] private Sprite activeButtonSprite;
    [SerializeField] private Sprite passiveButtonSprite;
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
        
        if (UserManager.Instance.Data.Level == 1)
        {
            levelText_tut.gameObject.SetActive(true);
            levelText.gameObject.SetActive(false);
        }
        else
        {
            levelText_tut.gameObject.SetActive(false);
            levelText.gameObject.SetActive(true);
            levelText.text = "Boss " + UserManager.Instance.Data.Level.ToString();
        }
        
        if (!UserManager.Instance.Data.SeenMergeTutorial)
        {
            if (UserManager.Instance.Data.UpgradeCount==1)
            {
                TutorialManager.Instance.ShowBuyHint();
            } else if(UserManager.Instance.Data.UpgradeCount==2)
            {
                TutorialManager.Instance.ShowBuyHint2();
            } 

            //این قسمت رو یک چک بزن 
            //playButton.interactable = false;
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
        int price = MergePlatform.Instance.GetCurrentUpgradePrice();
        foreach (TMP_Text priceText in priceTexts)
        {
            priceText.text = "$" + price.ToString();
        }

        if (price > UserManager.Instance.Data.Coins)
        {
            foreach (Image upgradeButton in upgradeButtons)
            {
                upgradeButton.sprite = passiveButtonSprite;
            }
        }
        else
        {
            int i = 0;
            foreach (Image upgradeButton in upgradeButtons)
            {
                if (i > 0 && !UserManager.Instance.Data.SeenMergeTutorial)
                {
                    upgradeButton.sprite = passiveButtonSprite;
                }
                else
                {
                    upgradeButton.sprite = activeButtonSprite;
                }
                i++;
            }
        }
    }

    public void OnClick_Play()
    {
        if (!UserManager.Instance.Data.SeenAssembleTutorial)
            return;
        GameManager.Instance.GoToPlayMode();
    }

    public void OnClick_Upgrade()
    {
        if (CanBuyUpgradeNow())
        {
            MergePlatform.Instance.SpawnUpgrade();
            CheckTut();
        }
    }
    
    public void OnClick_Upgrade_Tire()
    {
        if (!UserManager.Instance.Data.SeenMergeTutorial)
        {
            return;
        }

        if (CanBuyUpgradeNow())
        {
            MergePlatform.Instance.SpawnUpgrade(UpgradeType.Tire);
            CheckTut();
        }
    }
    
    public void OnClick_Upgrade_Turbo()
    {
        if (!UserManager.Instance.Data.SeenMergeTutorial)
        {
            return;
        }
        
        if (CanBuyUpgradeNow())
        {
            MergePlatform.Instance.SpawnUpgrade(UpgradeType.Turbo);
            CheckTut();
        }
    }
    
    public void OnClick_Upgrade_Bomb()
    {
        if (!UserManager.Instance.Data.SeenMergeTutorial)
        {
            return;
        }
        
        if (CanBuyUpgradeNow())
        {
            MergePlatform.Instance.SpawnUpgrade(UpgradeType.Bomb);
            CheckTut();
        }
    }
    
    public void OnClick_Upgrade_Gun()
    {
        if (CanBuyUpgradeNow())
        {
            MergePlatform.Instance.SpawnUpgrade(UpgradeType.Gun);
            CheckTut();
        }
    }

    private void CheckTut()
    {
        if (!UserManager.Instance.Data.SeenMergeTutorial && UserManager.Instance.Data.UpgradeCount==2)
        {
            TutorialManager.Instance.ShowBuyHint2();
        }
    }

    private bool CanBuyUpgradeNow()
    {
        if (CanContinueToUpgrade() && UserManager.Instance.Data.Coins >= MergePlatform.Instance.GetCurrentUpgradePrice())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    

    private bool CanContinueToUpgrade()
    {
        if (!UserManager.Instance.Data.SeenAssembleTutorial)
        {
            if (!UserManager.Instance.Data.SeenMergeTutorial)
            {
                if (MergePlatform.Instance.NumberOfFullCells() >= 2)
                {
                    return false;
                }
            }
            else
            {
                if (MergePlatform.Instance.NumberOfFullCells() >= 1)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
