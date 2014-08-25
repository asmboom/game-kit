using System.Collections.Generic;
using UnityEngine;

public abstract class PurchasableItem : VirtualItem
{
    [SerializeField]
    public Purchase PrimaryPurchase;

    [SerializeField]
    public Purchase SecondaryPurchase;

    public void Purchase()
    {
        Purchase(true);
    }

    public void Purchase(bool usePrimaryPurchase)
    {
        if (CanPurchaseNow())
        {
            if (usePrimaryPurchase)
            {
                PrimaryPurchase.Execute(this, Storage);
            }
            else
            {
                SecondaryPurchase.Execute(this, Storage);
            }
        }
    }

    public abstract bool CanPurchaseNow();
}
