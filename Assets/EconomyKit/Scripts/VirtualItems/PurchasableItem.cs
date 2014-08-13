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

    public void Purchase(int purchaseIndex)
    {
        purchaseIndex = Mathf.Max(0, purchaseIndex);
        if (purchaseIndex < PurchaseInfo.Count)
        {
            if (CanPurchaseNow())
            {
                PurchaseInfo[purchaseIndex].Run(this, Storage);
            }
        }
        else
        {
            Debug.LogError("Item [" + ID + "] doesn't have purchase of index [" + purchaseIndex + "]");
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
