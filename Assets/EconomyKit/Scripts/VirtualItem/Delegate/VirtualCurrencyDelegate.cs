public class VirtualCurrencyDelegate : VirtualItemDelegate
{
    public override bool IsPurchasableType { get { return false; } }

    public override bool HasBalance { get { return true; } }

    public override void Buy(VirtualItem item, Purchase purchase) { }

    public override void Give(VirtualItem item, int amount)
    {
        Storage.SetItemBalance(item.ID, Storage.GetItemBalance(item.ID) + amount);
    }

    public override bool CanBuyNow(VirtualItem item) { return false; }

    public override void Take(VirtualItem item, int amount)
    {
        Storage.SetItemBalance(item.ID, Storage.GetItemBalance(item.ID) - amount);
    }

    public override int GetCurrentLevel(VirtualItem item)
    {
        return 0;
    }
}