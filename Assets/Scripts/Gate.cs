using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gate : MonoBehaviour
{
    [SerializeField] private Image bg;
    [SerializeField] private Image labelBG;
    [SerializeField] private TMP_Text txt;

    [SerializeField] private Sprite positiveLabel;
    [SerializeField] private Sprite positiveBG;
    [SerializeField] private Sprite negetiveLabel;
    [SerializeField] private Sprite negetiveBG;

    public void Init(bool isNegetive)
    {
        bg.sprite = isNegetive ? negetiveBG : positiveBG;
        labelBG.sprite = isNegetive ? negetiveLabel : positiveLabel;
    }
}
