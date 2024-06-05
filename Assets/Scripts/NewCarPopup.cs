using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewCarPopup : MonoBehaviour
{
    public static NewCarPopup Instance;

    [SerializeField] private GameObject content;
    [SerializeField] private Image icon;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Hide();
    }

    public void Show(Sprite sprite)
    {
        content.SetActive(true);
        icon.sprite = sprite;
    }

    public void Hide()
    {
        content.SetActive(false);
    }
}
