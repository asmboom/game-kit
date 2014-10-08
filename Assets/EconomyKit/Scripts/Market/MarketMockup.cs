using System;
using UnityEngine;

namespace Beetle23
{
    public class MarketMockup : Market
    {
        protected override void RequestProductList()
        {
            _marketProducts = MarketProduct.CreateProductListFromVirtualItemsConfig(EconomyKit.Config);
            EndProductListRequest(true);
        }

        protected override void PurchaseProduct(MarketProduct product, int quantity)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log("Cost real currency" + product.FormattedPrice +
                "x" + quantity + " and purchased product [" + product.ProductIdentifier + "] named [" + product.Title + "]");
            EndPurchase(true);
#else
        EndPurchase(false);
#endif
        }
    }
}