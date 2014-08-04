using System.Collections.Generic;

public class MarketProduct
{
    public string productIdentifier { get; private set; }
    public string title { get; private set; }
    public string description { get; private set; }
    public string price { get; private set; }
    public string currencySymbol { get; private set; }
    public string currencyCode { get; private set; }
    public string formattedPrice { get; private set; }

    public static Dictionary<string, MarketProduct> CreateProductListFromStoreConfig(StoreConfig storeConfig)
    {
        Dictionary<string, MarketProduct> list = new Dictionary<string, MarketProduct>();

        for (int i = 0; i < storeConfig.Items.Count; i++)
        {
            MarketProduct product = TryCreateMarketProductFromVirtualItem(storeConfig.Items[i]);
            if (product != null)
            {
                list.Add(product.productIdentifier, product);
            }
        }

        return list;
    }

    public static MarketProduct TryCreateMarketProductFromVirtualItem(VirtualItem item)
    {
        if (item.IsPurchasable)
        {
            for (int i = 0; i < item.PurchaseInfo.Count; i++)
            {
                Purchase purchase = item.PurchaseInfo[i];
                if (purchase.IsMarketPurchase)
                {
                    MarketProduct product = new MarketProduct();
                    product.productIdentifier = item.ID;
                    product.title = item.Name;
                    product.price = purchase.Price.ToString();
                    product.description = item.Description;
                    product.currencySymbol = "￥";
                    product.currencyCode = "RMB";
                    product.formattedPrice = string.Format("{0}{1}.00", product.currencySymbol, product.price);
                    return product;
                }
            }
        }
        return null;
    }
}