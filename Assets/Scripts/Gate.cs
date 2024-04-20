using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gate : MonoBehaviour
{
    [SerializeField] private Image bg;
    [SerializeField] private Image labelBG;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text value;
    [SerializeField] private string valueText;

    [SerializeField] private Sprite positiveLabel;
    [SerializeField] private Sprite positiveBG;
    [SerializeField] private Sprite negetiveLabel;
    [SerializeField] private Sprite negetiveBG;
    
    [SerializeField] private Sprite[] levels;

    public void Init(bool isNegetive, int diff, int level)
    {
        bg.sprite = isNegetive ? negetiveBG : positiveBG;
        labelBG.sprite = isNegetive ? negetiveLabel : positiveLabel;
        value.text = string.Format(valueText, isNegetive ? "-" : "+", diff.ToString());
        icon.sprite = levels[level];
    }
}
