using System.Collections.Generic;
using UnityEngine;

namespace Beetle23
{
    public abstract class PurchasableItem : VirtualItem
    {
        [SerializeField]
        public List<Purchase> PurchaseInfo;

        public bool IsAffordable()
        {
            if (PurchaseInfo.Count > 0)
            {
                return IsAffordable(0);
            }
            else
            {
                // if the item doesn't contain any purchase, it is free
                return true;
            }
        }

        public bool IsAffordable(int index)
        {
            if (index < PurchaseInfo.Count)
            {
                Debug.Log(PurchaseInfo);
                return PurchaseInfo[index].IsMarketPurchase ||
                    PurchaseInfo[index].VirtualCurrency.Balance >= PurchaseInfo[index].Price;
            }
            else
            {
                Debug.LogError("Item [" + ID + "] doesn't have purchase of index " + index);
                return false;
            }
        }

        public PurchaseError Purchase()
        {
            if (PurchaseInfo.Count > 0)
            {
                return Purchase(0);
            }
            else
            {
                // if the item doesn't contain any purchase, it is free
                Give(1);
                return PurchaseError.None;
            }
        }

        public PurchaseError Purchase(int index)
        {
            if (index < PurchaseInfo.Count)
            {
                if (CanPurchaseNow())
                {
                    return PurchaseInfo[index].Execute(this);
                }
                return PurchaseError.NotAvailabe;
            }
            else
            {
                Debug.LogError("Item [" + ID + "] doesn't have purchase of index " + index);
                return PurchaseError.InvalidPurchase;
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
}
