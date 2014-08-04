using System;
using UnityEngine;

public class MarketMockup : Market
{
    protected override void RequestProductList()
    {
        _marketProducts = MarketProduct.CreateProductListFromStoreConfig(StoreKit.Config);
        EndProductListRequest(true);
    }

    protected override void PurchaseProduct(MarketProduct product, int quantity, int virtualCurrencyCount)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Log("Cost real currency" + product.formattedPrice +
            "x" + quantity + " and purchased product [" + product.productIdentifier + "] named [" + product.title + "]");
        EndPurchase(true);
#else
        EndPurchase(false);
#endif
    }
}
