using UnityEngine;

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
    // if purchasing with market, this is the market product id;
    // if purchaing with virtual currency, this is the virutal currency item id
    public string AssociatedID;

    public bool IsMarketPurchase
    {
        get
        {
            return Type == PurchaseType.PurchaseWithMarket;
        }
    }

    public PurchaseError Buy(VirtualItem item, IStoreStorage storage)
    {
        if (Type == PurchaseType.PurchaseWithVirtualCurrency)
        {
            return BuyWithVirtualCurrency(item, storage);
        }
        else
        {
            return BuyWithMarket(item, storage);
        }
    }

    private PurchaseError BuyWithVirtualCurrency(VirtualItem item, IStoreStorage storage)
    {
        StoreEvents.OnPurchaseStarted(item);

        int priceInVirtualCurrency = (int)Price;
        int balance = storage.GetItemBalance(AssociatedID);
        if (balance < priceInVirtualCurrency)
        {
            return PurchaseError.InsufficientVirtualCurrency;
        }
        else
        {
            StoreKit.Config.GetItemByID(AssociatedID).Take(priceInVirtualCurrency);
            item.Give(1);
            StoreEvents.OnPurchaseSucceeded(item);
            return PurchaseError.None;
        }
    }

    private PurchaseError BuyWithMarket(VirtualItem item, IStoreStorage storage)
    {
        StoreEvents.OnPurchaseStarted(item);

        _currentItemPurchasedWithMarket = item;
        Market.Instance.OnPurchaseSucceeded = OnMarketPurchaseSucceeded;
        Market.Instance.OnPurchaseFailed = OnMarketPurchaseFailed;
        return Market.Instance.StartPurchase(AssociatedID, 1, item.GainedVirtualCurrencyCountAfterPurchase);
    }

    private void OnMarketPurchaseSucceeded()
    {
        if (_currentItemPurchasedWithMarket != null)
        {
            _currentItemPurchasedWithMarket.Give(1);
            StoreEvents.OnPurchaseSucceeded(_currentItemPurchasedWithMarket);
        }
        _currentItemPurchasedWithMarket = null;
    }

    private void OnMarketPurchaseFailed()
    {
        StoreEvents.OnPurchaseFailed(_currentItemPurchasedWithMarket);
        _currentItemPurchasedWithMarket = null;
    }

    private static VirtualItem _currentItemPurchasedWithMarket = null;
}