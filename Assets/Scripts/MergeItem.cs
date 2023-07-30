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
        StartCoroutine(moveToCell(cell, speed, onEnd));
    }

    public void SetCurrentCell(MergeCell cell)
    {
        CurrentCell = cell;
    }

    private IEnumerator moveToCell(MergeCell cell, float speed, Action onEnd = null)
    {
        Vector3 direction = (cell.transform.position - transform.position).normalized;

        while (true)
        {
            float delta = speed * Time.deltaTime;
            if (Vector3.Distance(transform.position, cell.transform.position) > delta)
            {
                transform.Translate(direction*delta);
                yield return new WaitForEndOfFrame();
            }
            else
            {
                transform.position = cell.transform.position;
                break;
            }
        }
        
        onEnd?.Invoke();
    }
}
