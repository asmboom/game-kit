using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemAmountPair
{
    public VirtualItemBase Item;
    public int Amount;
}

public class VirtualItemPack : PurchasableItem
{
    [SerializeField]
    public List<ItemAmountPair> Items;

    protected override bool CanPurchaseNow()
    {
        return true;
    }

    protected override void TakeBalance(int amount)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            Items[i].Item.Take(amount);
        }
    }

    protected override void GiveBalance(int amount)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            Items[i].Item.Give(amount);
        }
    }
}