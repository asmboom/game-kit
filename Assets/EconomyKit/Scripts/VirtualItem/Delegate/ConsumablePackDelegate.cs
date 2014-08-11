public class ConsumablePackDelegate : VirtualItemDelegate
{
    public override bool IsPurchasableType { get { return true; } }

    public override bool HasBalance { get { return false; } }

    public override void Buy(VirtualItem item, Purchase purchase)
    {
        purchase.Buy(item, Storage);
    }

    public override bool CanBuyNow(VirtualItem item) { return true; }

    public override void Give(VirtualItem item, int amount)
    {
        Storage.SetItemBalance(item.RelatedItemID, 
            Storage.GetItemBalance(item.RelatedItemID) + item.RelatedItemAmount * amount);
    }

    public override void Take(VirtualItem item, int amount)
    {
        Storage.SetItemBalance(item.RelatedItemID,
            Storage.GetItemBalance(item.RelatedItemID) - item.RelatedItemAmount * amount);
    }

    public override int GetGainedVirtualCurrencyCountAfterPurchase(VirtualItem item)
    {
        return 0;
    }

    public override int GetCurrentLevel(VirtualItem item)
    {
        return 0;
    }
}