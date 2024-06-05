using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinParticle : MonoBehaviour
{
    [SerializeField] private GameObject go;
    [SerializeField] private TMP_Text txt;

    private void OnEnable()
    {
        go.SetActive(false);
    }

    public void Init(int amount)
    {
        go.SetActive(true);
   
        txt.text = "+" + amount.ToString();
    }
}
