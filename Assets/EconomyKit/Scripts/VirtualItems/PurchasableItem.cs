using System.Collections.Generic;
using UnityEngine;

public abstract class PurchasableItem : VirtualItem
{
    [SerializeField]
    public List<Purchase> PurchaseInfo;

    public void Purchase()
    {
        if (PurchaseInfo.Count > 0)
        {
            Purchase(0);
        }
        else
        {
            // if the item doesn't contain any purchase, it is free
            Give(1);
        }
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

    protected override void OnEnable()
    {
        base.OnEnable();

        if (PurchaseInfo == null)
        {
            PurchaseInfo = new List<Purchase>();
        }
    }
}
