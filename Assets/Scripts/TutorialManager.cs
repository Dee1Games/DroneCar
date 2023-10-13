using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] private HandUI hand;
    [SerializeField] private GameObject buyUpgrade;
    [SerializeField] private GameObject buyUpgrade2;
    [SerializeField] private GameObject merge;
    [SerializeField] private GameObject assemble;
    [SerializeField] private GameObject play;
    [SerializeField] private GameObject move;
    [SerializeField] private GameObject fly;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void Hide()
    {
        buyUpgrade.SetActive(false);
        buyUpgrade2.SetActive(false);
        play.SetActive(false);
        merge.SetActive(false);
        assemble.SetActive(false);
        move.SetActive(false);
        fly.SetActive(false);
    }

    public void HideHand()
    {
        hand.gameObject.SetActive(false);
    }

    public void ShowBuyHint()
    {
        Hide();
        hand.gameObject.SetActive(false);
        buyUpgrade.SetActive(true);
    }

    public void ShowBuyHint2()
    {
        Hide();
        hand.gameObject.SetActive(false);
        buyUpgrade2.SetActive(true);
    }

    public void ShowPlayHint()
    {
        Hide();
        hand.gameObject.SetActive(false);
        play.SetActive(true);
    }

    public void ShowMergeHint()
    {
        Hide();
        merge.SetActive(true);
    }

    public void ShowAssembleHint()
    {
        Hide();
        assemble.SetActive(true);
    }

    public void ShowMoveHint()
    {
        Hide();
        move.SetActive(true);
    }

    public void ShowFlyHint()
    {
        Hide();
        fly.SetActive(true);
    }
    
    public void ShowHand(Vector3 a , Vector3 b)
    {
        hand.gameObject.SetActive(true);
        hand.Init(a,b);
    }
}
