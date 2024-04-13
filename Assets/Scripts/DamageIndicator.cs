using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] private GameObject go;
    [SerializeField] private RectTransform rect;
    [SerializeField] private float scaleMultiplier = 1f;
    [SerializeField] private Vector2 scaleMultiplierLimit;
    [SerializeField] private TMP_Text txt;

    private void OnEnable()
    {
        go.SetActive(false);
    }

    public void Init(float damage)
    {
        go.SetActive(true);
        float fullHelath = LevelManager.Instance.CurrentLevelData.MonsterData.Health;
        float percent = damage / fullHelath;
        
        //txt.text = "-" + damage.ToString();
        float num = percent * 100f;
        string str = num.ToString("F2");
        if (num == 0)
        {
            str = "0.1";
        }
        txt.text = "-" + damage.ToString("F2");
        
        float distance = 1f;
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            distance = Vector3.Distance(transform.position, GameManager.Instance.Player.transform.position);
        }
        float scale = scaleMultiplier * distance;
        scale = Mathf.Clamp(scale, scaleMultiplierLimit.x, scaleMultiplierLimit.y);
        rect.localScale = Vector3.one * scale;
    }
}
