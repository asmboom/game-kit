using System.Collections.Generic;

namespace Beetle23
{
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

            foreach (var item in config.VirtualItems)
            {
                MarketProduct product = TryCreateMarketProductFromVirtualItem(item);
                if (product != null)
                {
                    list.Add(product.ProductIdentifier, product);
                }
            }

            return list;
        }

        public static MarketProduct TryCreateMarketProductFromVirtualItem(VirtualItem item)
        {
            if (item is PurchasableItem)
            {
                PurchasableItem purchasableItem = item as PurchasableItem;
                foreach (Purchase purchase in purchasableItem.PurchaseInfo)
                {
                    if (purchase.IsMarketPurchase)
                    {
                        MarketProduct product = new MarketProduct();
                        product.ProductIdentifier = purchase.MarketID;
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
}