using System.Collections;
using System.Collections.Generic;
using HomaGames.HomaBelly;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonUI : MonoBehaviour
{    
    public UpgradeType type;
    [SerializeField] private Sprite activeBtnBG;
    [SerializeField] private Sprite passiveBtnBG;
    [SerializeField] private Image btnBG;
    [SerializeField] private Image icon;
    [SerializeField] private Image coinIcon;
    [SerializeField] private TMP_Text priceTxt;
    [SerializeField] private TMP_Text valueTxt;
    [SerializeField] private TMP_Text titleTxt;
    [SerializeField] private MenuScreen screen;

    private bool isActive = false;
    private int lvl = 0;
    private int pr = 0;

    public void Init(int price, string value, bool active)
    {
        isActive = active;
        btnBG.sprite = active ? activeBtnBG : passiveBtnBG;
        coinIcon.gameObject.SetActive(true);
        //icon.sprite = sprite;
        priceTxt.text = price.ToString();
        valueTxt.text = "+" + value + "%";
        pr = price;
    }

    public void SetMax()
    {
        isActive = false;
        coinIcon.gameObject.SetActive(false);
        btnBG.sprite = passiveBtnBG;
        priceTxt.text = "MAX";
        valueTxt.text = "";
    }

    public void Upgrade()
    {
        if(!isActive)
            return;
        
        int l = GameManager.Instance.Player.GetUpgradeLevel(type);
        int newL = l + 1;

        if (GameManager.Instance.Player.SetUpgrade(type, newL))
        {
            GameManager.Instance.Player.ShowUpgradeVisuals();
            GameManager.Instance.Player.PlayUpgradeParticle();
        }
        
        UserManager.Instance.AddCoins(-pr);
        Analytics.ResourceFlowEvent(
            ResourceFlowType.Sink,
            "coin",
            pr,
            UserManager.Instance.Data.Coins,
            "coin",
            "upgrade",
            ResourceFlowReason.Progression
        );
        Analytics.ItemUpgraded(ItemUpgradeType.Upgrade, type.ToString(), newL, ItemFlowReason.Progression);
        
        UserManager.Instance.NextUpgrade();
        screen.Show();
    }
}
