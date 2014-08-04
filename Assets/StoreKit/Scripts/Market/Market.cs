using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Market
{
    private static Market _instance;
    public static Market Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = StoreKit.CreateMarket();
                _instance.SubscribeEvents();
            }
            return _instance;
        }
    }

    public delegate void VoidCallback();
    public Action<Dictionary<string, MarketProduct>> OnGetProductListSucceeded { get; set; }
    public VoidCallback OnGetProductListFailed { get; set; }
    public VoidCallback OnPurchaseSucceeded { get; set; }
    public VoidCallback OnPurchaseFailed { get; set; }

    public bool IsRequestingProduct { get; protected set; }
    public bool IsTransactionPending { get; protected set; }

    public bool IsProductListLoaded { get { return _marketProducts != null; } }

    public Dictionary<string, MarketProduct> ProductList { get { return _marketProducts; } }

    public void StartProductListRequest()
    {
        IsRequestingProduct = true;

        RequestProductList();
    }

    public PurchaseError StartPurchase(string productIdentifier, int quantity, int virtualCurrencyCount)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return PurchaseError.NoNetwork;
        }
        else if (IsTransactionPending)
        {
            return PurchaseError.TransactionPending;
        }
        else
        {
            IsTransactionPending = true;

            MarketProduct product = null;
            if (_marketProducts.TryGetValue(productIdentifier, out product))
            {
                PurchaseProduct(product, quantity, virtualCurrencyCount);
                return PurchaseError.None;
            }
            else
            {
                Debug.LogError("The purchasing product [" + productIdentifier + "] doesn't exist");
                EndPurchase(false);
                return PurchaseError.InvalidProductId;
            }
        }
    }

    public virtual void SubscribeEvents() { }
    protected abstract void RequestProductList();
    protected abstract void PurchaseProduct(MarketProduct product, int quantity, int virtualCurrencyCount);

    // derived class needs to call this function to end product list request
    protected void EndProductListRequest(bool success)
    {
        IsRequestingProduct = false;

        if (success)
        {
            if (OnGetProductListSucceeded != null)
            {
                OnGetProductListSucceeded(_marketProducts);
            }
        }
        else
        {
            if (OnGetProductListFailed != null)
            {
                OnGetProductListFailed();
            }
        }
    }

    // derived class needs to call this function to end purchase
    protected void EndPurchase(bool success)
    {
        IsTransactionPending = false;

        if (success)
        {
            if (OnPurchaseSucceeded != null)
            {
                OnPurchaseSucceeded();
            }
        }
        else
        {
            if (OnPurchaseFailed != null)
            {
                OnPurchaseFailed();
            }
        }
    }

    protected Dictionary<string, MarketProduct> _marketProducts = null;
}