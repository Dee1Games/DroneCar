using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MergeItem : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public int Level
    {
        get;
        private set;
    }
    
    public UpgradeType Type
    {
        get;
        private set;
    }

    public void Init(int level, UpgradeType type)
    {
        Level = level;
        Type = type;
        text.text = level.ToString();
    }

    public MergeCell CurrentCell
    {
        get;
        private set;
    }
    
    public void MoveToCell(MergeCell cell, float speed, Action onEnd = null)
    {
        StartCoroutine(moveToPos(cell.transform.position, speed, onEnd));
    }
    
    public void MoveToPlayerVehicle(float speed, Action onEnd = null)
    {
        StartCoroutine(moveToPos(PlayerVehicle.Instance.transform.position, speed, onEnd));
    }

    public void SetCurrentCell(MergeCell cell)
    {
        CurrentCell = cell;
    }

    private IEnumerator moveToPos(Vector3 pos, float speed, Action onEnd = null)
    {
        Vector3 direction = (pos - transform.position).normalized;

        while (true)
        {
            float delta = speed * Time.deltaTime;
            if (Vector3.Distance(transform.position, pos) > delta)
            {
                transform.Translate(direction*delta);
                yield return new WaitForEndOfFrame();
            }
            else
            {
                transform.position = pos;
                break;
            }
        }
        
        onEnd?.Invoke();
    }
}
