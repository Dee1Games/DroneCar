using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gate : MonoBehaviour
{
    [SerializeField] private Image bg;
    [SerializeField] private Image labelBG;
    [SerializeField] private TMP_Text value;
    [SerializeField] private string valueText;

    [SerializeField] private Sprite positiveLabel;
    [SerializeField] private Sprite positiveBG;
    [SerializeField] private Sprite negetiveLabel;
    [SerializeField] private Sprite negetiveBG;

    public void Init(bool isNegetive, int diff)
    {
        bg.sprite = isNegetive ? negetiveBG : positiveBG;
        labelBG.sprite = isNegetive ? negetiveLabel : positiveLabel;
        value.text = string.Format(valueText, isNegetive ? "-" : "+", diff.ToString());
    }
}
