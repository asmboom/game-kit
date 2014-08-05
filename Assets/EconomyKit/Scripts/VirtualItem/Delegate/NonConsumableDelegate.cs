public class NonConsumableDelegate : VirtualItemDelegate
{
    public override bool IsPurchasableType { get { return true; } }

    public override bool HasBalance { get { return true; } }

    public override void Buy(VirtualItem item, Purchase purchase)
    {
        if (CanBuyNow(item))
        {
            purchase.Buy(item, Storage);
        }
    }

    public override bool CanBuyNow(VirtualItem item) { return Storage.GetItemBalance(item.ID) < 1; }

    public override void Give(VirtualItem item, int amount)
    {
        Storage.SetItemBalance(item.ID, 1);
    }

    public override void Take(VirtualItem item, int amount)
    {
        Storage.SetItemBalance(item.ID, 0);
    }

    public override int GetGainedVirtualCurrencyCountAfterPurchase(VirtualItem item)
    {
        return 0;
    }

    public override int GetCurrentLevel(VirtualItem item)
    {
        return Storage.GetGoodCurrentLevel(item.ID);
    }
}