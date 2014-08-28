using System.Collections.Generic;
using UnityEngine;

public abstract class PurchasableItem : VirtualItem
{
    [SerializeField]
    public List<Purchase> PurchaseInfo;

    public void Purchase()
    {
        Purchase(0);
    }

    public void Purchase(int index)
    {
        if (index < PurchaseInfo.Count)
        {
            if (CanPurchaseNow())
            {
                PurchaseInfo[index].Execute(this);
            }
        }
        else
        {
            Debug.LogError("Item [" + ID + "] doesn't have purchase of index " + index);
        }
    }

    public abstract bool CanPurchaseNow();
}
