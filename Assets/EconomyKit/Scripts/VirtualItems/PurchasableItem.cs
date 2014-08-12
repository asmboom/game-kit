using System.Collections.Generic;
using UnityEngine;

public abstract class PurchasableItem : VirtualItemBase
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
                PurchaseInfo[purchaseIndex].Exec(this, Storage);
            }
        }
        else
        {
            Debug.LogError("Item [" + ID + "] doesn't have purchase of index [" + purchaseIndex + "]");
        }
    }

    protected abstract bool CanPurchaseNow();

    protected virtual void OnEnable()
    {
        if (PurchaseInfo == null)
        {
            PurchaseInfo = new List<Purchase>();
        }
    }
}
