using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelIndicatorUI : MonoBehaviour
{
    public UpgradeType Type;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Transform Pos;


    private void Update()
    {
        transform.position = Pos.position;
        transform.LookAt(MergePlatform.Instance.CameraTransform.forward);
    }

    public void SetLevel(int level)
    {
        text.text = level.ToString();
    }
}
