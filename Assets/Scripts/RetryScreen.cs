using System.Collections;
using System.Collections.Generic;
using HomaGames.HomaBelly;
using TMPro;
using UnityEngine;

public class RetryScreen : MonoBehaviour
{

    [SerializeField] private TMP_Text btnText;

    public void Show()
    {
        btnText.text = "Play on " + GameManager.Instance.retryPrice.ToString();
    }

    public void OnClick_OK()
    {
        UserManager.Instance.AddCoins(GameManager.Instance.retryPrice);
        Analytics.ResourceFlowEvent(
            ResourceFlowType.Sink,
            "coin",
            GameManager.Instance.retryPrice,
            UserManager.Instance.Data.Coins,
            "coin",
            "life",
            ResourceFlowReason.Progression
        );
        UserManager.Instance.AddLife();
        GameManager.Instance.GoToUpgradeMode();
        gameObject.SetActive(false);
    }
    
    
    public void OnClick_Close()
    {
        Analytics.MissionFailed(UserManager.Instance.Data.Level.ToString());
        UserManager.Instance.ResetLife();
        UserManager.Instance.SetMonsterHealth(1f);
        UserManager.Instance.ResetRun();
        LevelManager.Instance.InitCurrentLevel();
        GameManager.Instance.GoToUpgradeMode();
        gameObject.SetActive(false);
    }
}
