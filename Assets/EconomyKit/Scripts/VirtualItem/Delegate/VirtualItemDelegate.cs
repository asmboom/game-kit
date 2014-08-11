using UnityEngine;

public abstract class VirtualItemDelegate
{
    public abstract bool IsPurchasableType { get; }
    public abstract bool HasBalance { get; }
    public abstract void Buy(VirtualItem item, Purchase purchase);
    public abstract bool CanBuyNow(VirtualItem item);
    public abstract void Give(VirtualItem item, int amount);
    public abstract void Take(VirtualItem item, int amount);
    public abstract int GetCurrentLevel(VirtualItem item);

    public int GetBalance(VirtualItem item)
    {
        return Storage.GetItemBalance(item.ID);
    }

    public void ResetBalance(VirtualItem item)
    {
        Storage.SetItemBalance(item.ID, 0);
    }

    protected static IEconomyStorage Storage
    {
        get
        {
            if (_storage == null)
            {
                _storage = EconomyKit.CreateStorage();
            }
            return _storage;
        }
    }

    private static IEconomyStorage _storage;
}

public class NullableVirtualItemDelegate : VirtualItemDelegate
{
    public override bool IsPurchasableType { get { return false; } }
    public override bool HasBalance { get { return false; } }
    public override void Buy(VirtualItem item, Purchase purchase) { }
    public override bool CanBuyNow(VirtualItem item) { return false; }
    public override void Give(VirtualItem item, int amount) { }
    public override void Take(VirtualItem item, int amount) { }
    public override int GetCurrentLevel(VirtualItem item) { return 0; }
}