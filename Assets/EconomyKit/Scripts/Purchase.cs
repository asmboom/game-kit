using UnityEngine;

namespace Beetle23
{
    public enum PurchaseType
    {
        PurchaseWithMarket,
        PurchaseWithVirtualCurrency
    }

    [System.Serializable]
    public class Purchase
    {
        public PurchaseType Type;
        public float Price;
        // if purchasing with market, this is market id
        // if purchasing with virtual currency, this is virtual currency id
        public string AssociatedID;

        public bool IsMarketPurchase
        {
            get
            {
                return Type == PurchaseType.PurchaseWithMarket;
            }
        }

        public VirtualCurrency VirtualCurrency
        {
            get
            {
                _virtualCurrency = EconomyKit.Config.PopulateItemIfNull(AssociatedID, 
                    _virtualCurrency) as VirtualCurrency;
                return _virtualCurrency;
            }   
        }

        public PurchaseError Execute(PurchasableItem purchasable)
        {
            if (Type == PurchaseType.PurchaseWithVirtualCurrency)
            {
                return BuyWithVirtualCurrency(purchasable);
            }
            else
            {
                return BuyWithMarket(purchasable);
            }
        }

        private PurchaseError BuyWithVirtualCurrency(PurchasableItem item)
        {
            EconomyKit.OnPurchaseStarted(item);

            int priceInVirtualCurrency = (int)Price;
            int balance = VirtualCurrency.Balance;
            if (balance < priceInVirtualCurrency)
            {
                return PurchaseError.InsufficientVirtualCurrency;
            }
            else
            {
                VirtualCurrency.Take(priceInVirtualCurrency);
                item.Give(1);
                EconomyKit.OnPurchaseSucceeded(item);
                return PurchaseError.None;
            }
        }

        private PurchaseError BuyWithMarket(PurchasableItem item)
        {
            EconomyKit.OnPurchaseStarted(item);

            _currentItemPurchasedWithMarket = item;
            return Market.Instance.StartPurchase(AssociatedID, 1,
                OnMarketPurchaseSucceeded, OnMarketPurchaseFailed);
        }

        private void OnMarketPurchaseSucceeded()
        {
            if (_currentItemPurchasedWithMarket != null)
            {
                _currentItemPurchasedWithMarket.Give(1);
                EconomyKit.OnPurchaseSucceeded(_currentItemPurchasedWithMarket);
            }
            _currentItemPurchasedWithMarket = null;
        }

        private void OnMarketPurchaseFailed()
        {
            EconomyKit.OnPurchaseFailed(_currentItemPurchasedWithMarket);
            _currentItemPurchasedWithMarket = null;
        }

        private VirtualCurrency _virtualCurrency;

        private static PurchasableItem _currentItemPurchasedWithMarket = null;
    }
}