using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MergeCell : MonoBehaviour
{
    public int Index;
    
    public MergeItem Item
    {
        get;
        private set;
    }
    
    public bool IsFull => (Item!=null);

    [SerializeField] private SellUI sellUI;

    public void Init()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<SellUI>() == null)
            {
                Destroy(child.gameObject);
            }
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
    }
    
    private void OnMouseDown()
    {
        MergePlatform.Instance.ItemSelected(Item);
        RemoveItem();
        UserManager.Instance.SetMergePlatformCell(Index, UpgradeType.Tire, -1);
    }

    public void ShowSellUI()
    {
        sellUI.gameObject.SetActive(true);
    }

    public void HideSellUI()
    {
        sellUI.gameObject.SetActive(false);
    }

    public void OnClick_Sell()
    {
        if (Item == null)
            return;
        DestroyImmediate(Item.gameObject);
        RemoveItem();
        UserManager.Instance.SetMergePlatformCell(Index, UpgradeType.Tire, -1);
        UserManager.Instance.AddCoins(Mathf.RoundToInt(0.8f * MergePlatform.Instance.GetCurrentUpgradePrice() * (Mathf.Pow(2, (UserManager.Instance.Data.Level-1)))));
        UIManager.Instance.Refresh();
        
    }
}
