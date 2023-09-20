using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MergeCell : MonoBehaviour
{
    [SerializeField] private GameObject sellUI;
    [SerializeField] private TMP_Text sellUIText;
    
    
    public int Index;
    
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
            if(child.GetComponent<MergeItem>())
                Destroy(child.gameObject);
        }
        RemoveItem();
    }

    public void SetItem(MergeItem item)
    {
        if (Item != null && Item.Type == item.Type && Item.Level == item.Level)
        {
            MergeItem newItem = Instantiate(GameManager.Instance.Player.Config.GetItem(Item.Type, Item.Level+1).MergePrefab, transform).GetComponent<MergeItem>();
            newItem.Init(Item.Level+1, Item.Type);
            Destroy(Item.gameObject);
            Destroy(item.gameObject);
            item = newItem;
        }
        Item = item;
        Item.transform.parent = transform;
        Item.transform.localPosition = Vector3.zero;
        Item.SetCurrentCell(this);
        UserManager.Instance.SetMergePlatformCell(Index, item.Type, item.Level);
    }
    
    public void RemoveItem()
    {
        Item = null;
        MergePlatform.Instance.ShowSellUIIfNeeded();
    }

    public void ShowSellUI()
    {
        sellUI.SetActive(true);
    }
    
    public void HideSellUI()
    {
        sellUI.SetActive(false);
    }
    
    private void OnMouseDown()
    {
        MergePlatform.Instance.ItemSelected(Item);
        RemoveItem();
        UserManager.Instance.SetMergePlatformCell(Index, UpgradeType.Tire, -1);
    }
    
    public void OnClick_Sell()
    {
        foreach (Transform child in transform)
        {
            if(child.GetComponent<MergeItem>())
                Destroy(child.gameObject);
        }
        RemoveItem();
        UserManager.Instance.SetMergePlatformCell(Index, UpgradeType.Tire, -1);
    }
}
