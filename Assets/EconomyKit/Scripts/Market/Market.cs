using System;
using System.Collections.Generic;
using UnityEngine;

namespace Beetle23
{
    public abstract class Market
    {
        private static Market _instance;
        public static Market Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = EconomyKit.CreateMarket();
                    _instance.SubscribeEvents();
                }
                return _instance;
            }
        }

        public bool IsRequestingProduct { get; protected set; }
        public bool IsTransactionPending { get; protected set; }

        public bool IsProductListLoaded { get { return _marketProducts != null; } }

        public Dictionary<string, MarketProduct> ProductList { get { return _marketProducts; } }

        public void StartProductListRequest(Action<Dictionary<string, MarketProduct>> onGetProductListSucceeded,
            Action onGetProductListFailed)
        {
            _onGetProductListSucceeded = onGetProductListSucceeded;
            _onGetProductListFailed = onGetProductListFailed;

            IsRequestingProduct = true;
            RequestProductList();
        }

        public PurchaseError StartPurchase(string productIdentifier, int quantity,
            Action onPurchaseSucceeded, Action onPurchaseFailed)
        {
            _onPurchaseSucceeded = onPurchaseSucceeded;
            _onPurchaseFailed = onPurchaseFailed;

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
                    PurchaseProduct(product, quantity);
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
        protected abstract void PurchaseProduct(MarketProduct product, int quantity);

        // derived class needs to call this function to end product list request
        protected void EndProductListRequest(bool success)
        {
            IsRequestingProduct = false;

            if (success)
            {
                if (_onGetProductListSucceeded != null)
                {
                    _onGetProductListSucceeded(_marketProducts);
                }
            }
            else
            {
                if (_onGetProductListFailed != null)
                {
                    _onGetProductListFailed();
                }
            }
        }

        // derived class needs to call this function to end purchase
        protected void EndPurchase(bool success)
        {
            IsTransactionPending = false;

            if (success)
            {
                if (_onPurchaseSucceeded != null)
                {
                    _onPurchaseSucceeded();
                }
            }
            else
            {
                if (_onPurchaseFailed != null)
                {
                    _onPurchaseFailed();
                }
            }
        }

        protected Dictionary<string, MarketProduct> _marketProducts = null;

        private Action<Dictionary<string, MarketProduct>> _onGetProductListSucceeded;
        private Action _onGetProductListFailed;
        private Action _onPurchaseSucceeded;
        private Action _onPurchaseFailed;
    }
}