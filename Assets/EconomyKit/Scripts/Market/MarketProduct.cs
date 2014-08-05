using System.Collections.Generic;

public class MarketProduct
{
    public string ProductIdentifier { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Price { get; private set; }
    public string CurrencySymbol { get; private set; }
    public string CurrencyCode { get; private set; }
    public string FormattedPrice { get; private set; }

    public static Dictionary<string, MarketProduct> CreateProductListFromVirtualItemsConfig(VirtualItemsConfig config)
    {
        Dictionary<string, MarketProduct> list = new Dictionary<string, MarketProduct>();

        for (int i = 0; i < config.Items.Count; i++)
        {
            MarketProduct product = TryCreateMarketProductFromVirtualItem(config.Items[i]);
            if (product != null)
            {
                list.Add(product.ProductIdentifier, product);
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
                    product.ProductIdentifier = purchase.AssociatedID;
                    product.Title = item.Name;
                    product.Price = purchase.Price.ToString();
                    product.Description = item.Description;
                    product.CurrencySymbol = "￥";
                    product.CurrencyCode = "RMB";
                    product.FormattedPrice = string.Format("{0}{1}.00", product.CurrencySymbol, product.Price);
                    return product;
                }
            }
        }
        return null;
    }
}