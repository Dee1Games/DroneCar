using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeCell : MonoBehaviour
{
    public MergeItem Item
    {
        get;
        private set;
    }
    
    public bool IsFull => (Item!=null);

    public void Init()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        RemoveItem();
    }

    public void SetItem(MergeItem item)
    {
        Item = item;
        Item.transform.parent = transform;
        Item.SetCurrentCell(this);
    }
    
    public void RemoveItem()
    {
        Item = null;
    }
    
    private void OnMouseDown()
    {
        MergePlatform.Instance.ItemSelected(Item);
        RemoveItem();
    }
}
