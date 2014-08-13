using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PackElement
{
    public string ItemID;
    public int Amount;

    public VirtualItem Item
    {
        get
        {
            return string.IsNullOrEmpty(ItemID) ? 
                null : EconomyKit.Config.GetItemByID(ItemID);
        }
    }

    public void Give(int amount)
    {
        Item.Give(amount * Amount);
    }

    public void Take(int amount)
    {
        Item.Take(amount * Amount);
    }
}

public class VirtualItemPack : PurchasableItem
{
    [SerializeField]
    public List<PackElement> PackElements;

    public override bool CanPurchaseNow()
    {
        return true;
    }

    protected override void TakeBalance(int amount)
    {
        for (int i = 0; i < PackElements.Count; i++)
        {
            PackElements[i].Take(amount);
        }
    }

    protected override void GiveBalance(int amount)
    {
        for (int i = 0; i < PackElements.Count; i++)
        {
            PackElements[i].Give(amount);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (PackElements == null)
        {
            PackElements = new List<PackElement>();
        }
    }
}