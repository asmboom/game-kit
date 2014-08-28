using UnityEngine;

public class UpgradeItem : PurchasableItem
{
    [SerializeField]
    public VirtualItem RelatedItem;

    public override bool CanPurchaseNow()
    {
        VirtualItem associatedItem = RelatedItem;
        VirtualItem nextUpgradeItem = associatedItem.NextUpgradeItem;
        return nextUpgradeItem != null && nextUpgradeItem == this;
    }

    protected override void TakeBalance(int amount)
    {
        Storage.SetGoodCurrentLevel(RelatedItem.ID,
            Storage.GetGoodCurrentLevel(RelatedItem.ID) - 1);
    }

    protected override void GiveBalance(int amount)
    {
        Storage.SetGoodCurrentLevel(RelatedItem.ID,
            Storage.GetGoodCurrentLevel(RelatedItem.ID) + 1);
    }
}