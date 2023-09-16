using System;
using System.Collections;
using System.Collections.Generic;
using SupersonicWisdomSDK;
using TMPro;
using UnityEngine;

public class InGameScreen : UIScreen
{
    public override UIScreenID ID => UIScreenID.InGame;

    [SerializeField] private HealthUI monsterHealthUI;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject retryButton;
    [SerializeField] private RectTransform monsterPointer;
    [SerializeField] private RectTransform monsterPointerArrow;

    private void OnEnable()
    {
        Monster.OnHealthChange += UpdateMonsterHealthBar;
        PlayerVehicle.OnExploded += HideRetryButton;
    }

    private void OnDisable()
    {
        Monster.OnHealthChange -= UpdateMonsterHealthBar;
        PlayerVehicle.OnExploded -= HideRetryButton;
    }

    public override void Init()
    {
        base.Init();
    }
    
    public override void Show()
    {
        base.Show();
        retryButton.SetActive(true);
        coinsText.text = UserManager.Instance.Data.Coins.ToString();
        levelText.text = "Boss " + UserManager.Instance.Data.Level.ToString();
    }

    private void Update()
    {
        RefreshMonsterPointer();
    }
    
    public override void Hide()
    {
        base.Hide();
    }
    
    public void OnClick_Back()
    {
        GameManager.Instance.GoToUpgradeMode();
    }

    private void RefreshMonsterPointer()
    {
        if (GameManager.Instance.Monster != null)
        {
            monsterPointer.gameObject.SetActive(true);
            float halfWidth = Screen.width / 2;
            float halfHeight = Screen.height / 2;
            Vector3 pos = Camera.main.WorldToScreenPoint(GameManager.Instance.Monster.GetCOMPos());
            monsterPointer.position = pos;
            float pointerHalfSize = monsterPointer.rect.width / 2f;
            Vector2 anchoredPos = monsterPointer.anchoredPosition;
            if (Mathf.Abs(anchoredPos.x) < Screen.width && Mathf.Abs(anchoredPos.y) < Screen.height && pos.z>0)
                
            {
                monsterPointer.gameObject.SetActive(false);
                return;
            }
            
            if (pos.z < 0)
                anchoredPos.y = -halfHeight + pointerHalfSize;
            if (anchoredPos.x < -halfWidth)
                anchoredPos.x = -halfWidth + pointerHalfSize;
            if (anchoredPos.x > halfWidth)
                anchoredPos.x = halfWidth - pointerHalfSize;
            if (anchoredPos.y < -halfHeight)
                anchoredPos.y = -halfHeight + pointerHalfSize;
            if (anchoredPos.y > halfHeight)
                anchoredPos.y = halfHeight - pointerHalfSize;
            if (Math.Abs(anchoredPos.x) < halfWidth - pointerHalfSize &&
                Math.Abs(anchoredPos.y) < halfHeight - pointerHalfSize)
            {
                monsterPointer.gameObject.SetActive(false);
            }
            else
            {
                monsterPointer.anchoredPosition = anchoredPos;

                Vector3 monsterPointerArrowAngles = monsterPointerArrow.eulerAngles;
                monsterPointerArrowAngles.z = Mathf.Atan(anchoredPos.y / anchoredPos.x) * (180 / Mathf.PI);
                if (anchoredPos.x < 0)
                    monsterPointerArrowAngles.z += 180;
                monsterPointerArrow.eulerAngles = monsterPointerArrowAngles;
            }
            
        }
        else
        {
            monsterPointer.gameObject.SetActive(false);
        }
    }
    
    public void OnClick_Retry()
    {
        GameManager.Instance.Player.Deactivate();
        Debug.Log($"Run {UserManager.Instance.Data.Run} Failed");
        try
        {
            SupersonicWisdom.Api.NotifyLevelFailed(UserManager.Instance.Data.Run, null);
        } catch {}
        GameManager.Instance.GoToPlayMode();
        Debug.Log($"Run {UserManager.Instance.Data.Run} Started");
        try
        {
            SupersonicWisdom.Api.NotifyLevelStarted(UserManager.Instance.Data.Run, null);
        } catch {}
    }

    private void UpdateMonsterHealthBar(float currentHealth, float maxHealth)
    {
        monsterHealthUI.UpdateHealthUI(currentHealth, maxHealth); 
    }

    private void HideRetryButton()
    {
        retryButton.SetActive(false);
    }
}
