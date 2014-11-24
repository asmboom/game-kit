using System.Collections.Generic;
using System;
using UnityEngine;

namespace Beetle23
{
    public abstract class PurchasableItem : VirtualItem
    {
		public Action OnPurchased = delegate { };

        [SerializeField]
        public List<Purchase> PurchaseInfo;

        public PurchasableItem()
        {
            PurchaseInfo = new List<Purchase>();
        }

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

        public override string ToString()
        {
            string final = string.Empty;
            if (PurchaseInfo.Count > 0)
            {
                for (var i = 0; i < PurchaseInfo.Count; i++)
                {
                    Purchase purchase = PurchaseInfo[i];
                    if (purchase != null)
                    {
                        if (i > 0)
                        {
                            final += "\nor ";
                        }
                        final += purchase.IsMarketPurchase ?
                            string.Format("Real money {0} ({1})", purchase.Price, purchase.MarketID) :
                            string.Format("{0}x{1}",
                                purchase.VirtualCurrency != null ? purchase.VirtualCurrency.Name : "null", purchase.Price);
                    }
                }
            }
            else
            {
                final = "Free";
            }
            return final;
        }

        public abstract bool CanPurchaseNow();
    }
}
