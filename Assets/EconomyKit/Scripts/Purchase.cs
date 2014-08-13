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

    public PurchaseError Run(PurchasableItem purchasable, IEconomyStorage storage)
    {
        if (Type == PurchaseType.PurchaseWithVirtualCurrency)
        {
            return BuyWithVirtualCurrency(purchasable, storage);
        }
        else
        {
            return BuyWithMarket(purchasable, storage);
        }
    }

    private PurchaseError BuyWithVirtualCurrency(PurchasableItem item, IEconomyStorage storage)
    {
        EconomyKit.OnPurchaseStarted(item);

        int priceInVirtualCurrency = (int)Price;
        int balance = storage.GetItemBalance(AssociatedID);
        if (balance < priceInVirtualCurrency)
        {
            return PurchaseError.InsufficientVirtualCurrency;
        }
        else
        {
            EconomyKit.Config.GetItemByID(AssociatedID).Take(priceInVirtualCurrency);
            item.Give(1);
            EconomyKit.OnPurchaseSucceeded(item);
            return PurchaseError.None;
        }
    }

    private PurchaseError BuyWithMarket(PurchasableItem item, IEconomyStorage storage)
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

    private static PurchasableItem _currentItemPurchasedWithMarket = null;
}