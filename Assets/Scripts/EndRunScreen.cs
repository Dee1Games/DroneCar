using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndRunScreen : UIScreen
{
    public override UIScreenID ID => UIScreenID.EndRun;

    [SerializeField] private float skipDur;
    [SerializeField] private HealthUI monsterHealthUI;
    [SerializeField] private Image monsterHealthSliderFill;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text monsterHealthUIText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private GameObject beforGO;
    [SerializeField] private GameObject nowGO;
    [SerializeField] private GameObject afterGO;
    [SerializeField] private GameObject progressBarGo;
    [SerializeField] private GameObject deadTextGo;
    [SerializeField] private GameObject crossGo;
    [SerializeField] private Image beforImage;
    [SerializeField] private Image nowImage;
    [SerializeField] private Image afterImage;
    
    public override void Init()
    {
        base.Init();
    }
    
    public override void Show()
    {
        base.Show();
        Animate();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void OnClick_Continue()
    {
        // if (UserManager.Instance.Data.Level == 1)
        // {
        //     if (!TutorialManager.Instance.HasSeenLimb2())
        //     {
        //         GameManager.Instance.GoToPlayMode();
        //     }
        //     else
        //     {
        //         GameManager.Instance.GoToUpgradeMode();
        //     }
        // }
        // else
        {
            GameManager.Instance.GoToUpgradeMode();
        }
    }

    private void Animate()
    {
        StartCoroutine(animate());
    }

    private IEnumerator animate()
    {
        if (GameManager.Instance.CurrentRunDamage == 0f && GameManager.Instance.RunResult!=RunResult.Died)
        {
            GameManager.Instance.RunResult = RunResult.Missed;
        }
        
        if (GameManager.Instance.RunResult == RunResult.Died)
        {
            titleText.text = "Try Again";
        } else if (GameManager.Instance.RunResult == RunResult.Missed)
        {
            titleText.text = "You Missed!";
        } else if (GameManager.Instance.RunResult == RunResult.Hit)
        {
            titleText.text = "Excellent Shot!";
        }
        else if (GameManager.Instance.RunResult == RunResult.Finish)
        {
            titleText.text = "Boss Is Done";
        }
        
        crossGo.SetActive(false);
        deadTextGo.SetActive(false);
        progressBarGo.SetActive(true);
        
        beforGO.SetActive(true);
        if (LevelManager.Instance.PreviousLevelData != null)
        {
            beforImage.sprite = LevelManager.Instance.PreviousLevelData.MonsterData.Sprite;
        }
        else
        {
            beforGO.SetActive(false);
        }
        nowImage.sprite = LevelManager.Instance.CurrentLevelData.MonsterData.Sprite;
        afterImage.sprite = LevelManager.Instance.NextLevelData.MonsterData.Sprite;
        

        rewardText.text = "+ 0";
        levelText.text = "Boss " + UserManager.Instance.Data.Level.ToString();
        coinText.text = UserManager.Instance.Data.Coins.ToString();
        
        float beforeHealth = UserManager.Instance.Data.MonsterHealth + (GameManager.Instance.CurrentRunDamage/LevelManager.Instance.CurrentLevelData.MonsterData.Health);
        float afterHealth = Mathf.Clamp(UserManager.Instance.Data.MonsterHealth, 0f, 1f);
        

        monsterHealthSliderFill.fillAmount = beforeHealth;
        monsterHealthUI.SetHealth(beforeHealth);
        monsterHealthUIText.text = $"{Mathf.FloorToInt(beforeHealth*100f)}% Left";
        
        yield return new WaitForSeconds(0.5f);


        float dur = 1.5f;
        float t = 0f;
        while (t < dur)
        {
            float nowHealth = Mathf.Lerp(beforeHealth, afterHealth, t);
            
            monsterHealthSliderFill.fillAmount = nowHealth;
            monsterHealthUI.SetHealth(nowHealth);
            monsterHealthUIText.text = $"{Mathf.FloorToInt(nowHealth*100f)}% Left";
            
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }
        
        monsterHealthSliderFill.fillAmount = afterHealth;
        monsterHealthUI.SetHealth(afterHealth);
        monsterHealthUIText.text = $"{Mathf.FloorToInt(afterHealth*100f)}% Left";

        if (GameManager.Instance.RunResult == RunResult.Finish)
        {
            crossGo.SetActive(true);
            deadTextGo.SetActive(true);
            progressBarGo.SetActive(false);
        }

        int c = LevelManager.Instance.GetPreviousRunReward();
        
        if (GameManager.Instance.RunResult == RunResult.Missed || GameManager.Instance.CurrentRunDamage == 0f)
        {
            c = 0;
        }

        if (c != 0)
        {
            dur = 1.5f;
            t = 0f;
            while (t < dur)
            {
                float nowReward = Mathf.Lerp(0, c, t);
                rewardText.text = "+ " + Mathf.FloorToInt(nowReward).ToString();
            
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
        }

        rewardText.text = "+ " + c.ToString();
        UserManager.Instance.AddCoins(c);

        yield return new WaitForSeconds(skipDur);
        
        if (GameManager.Instance.RunResult == RunResult.Finish)
        {
            UserManager.Instance.ResetAllVehicleUpgrades();
            UserManager.Instance.NextLevel();
        }
        
        OnClick_Continue();
    }
}
