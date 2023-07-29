using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeItem : MonoBehaviour
{
    public MergeCell CurrentCell
    {
        get;
        private set;
    }
    
    public void MoveToCell(MergeCell cell, float speed)
    {
        StartCoroutine(moveToCell(cell, speed));
    }

    public void SetCurrentCell(MergeCell cell)
    {
        CurrentCell = cell;
    }

    private IEnumerator moveToCell(MergeCell cell, float speed)
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
        
    }
}
