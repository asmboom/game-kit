public class ConsumableDelegate : VirtualItemDelegate
{
    public override bool IsPurchasableType { get { return true; } }

    public override bool HasBalance { get { return true; } }

    public override void Buy(VirtualItem item, Purchase purchase)
    {
        purchase.Buy(item, Storage);
    }

    public override bool CanBuyNow(VirtualItem item) { return true; }

    public override void Give(VirtualItem item, int amount)
    {
        Storage.SetItemBalance(item.ID, Storage.GetItemBalance(item.ID) + amount);
    }

    public override void Take(VirtualItem item, int amount)
    {
        Storage.SetItemBalance(item.ID, Storage.GetItemBalance(item.ID) - amount);
    }

    public override int GetCurrentLevel(VirtualItem item)
    {
        return Storage.GetGoodCurrentLevel(item.ID);
    }
}