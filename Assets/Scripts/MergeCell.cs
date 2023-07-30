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
        if (Item != null && Item.Type == item.Type && Item.Level == item.Level)
        {
            MergeItem newItem = Instantiate(PlayerVehicle.Instance.Config.GetItem(Item.Type, Item.Level+1).MergePrefab, transform).GetComponent<MergeItem>();
            newItem.Init(Item.Level+1, Item.Type);
            Destroy(Item.gameObject);
            Destroy(item.gameObject);
            item = newItem;
        }
        Item = item;
        Item.transform.parent = transform;
        Item.transform.localPosition = Vector3.zero;
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
