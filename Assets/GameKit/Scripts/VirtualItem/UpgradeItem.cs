using UnityEngine;

namespace Beetle23
{
    [System.Serializable]
    public class UpgradeItem : PurchasableItem
    {
        public UpgradeItem(VirtualItem relatedItem)
        {
            RelatedItem = relatedItem;
        }

        public VirtualItem RelatedItem { get; private set; }

        public override bool CanPurchaseNow()
        {
            VirtualItem associatedItem = RelatedItem;
            VirtualItem nextUpgradeItem = associatedItem.NextUpgradeItem;
            return nextUpgradeItem != null && nextUpgradeItem == this;
        }

        protected override void DoTake(int amount)
        {
            VirtualItemStorage.SetGoodCurrentLevel(RelatedItem.ID,
                VirtualItemStorage.GetGoodCurrentLevel(RelatedItem.ID) - 1);
        }

        protected override void DoGive(int amount)
        {
            VirtualItemStorage.SetGoodCurrentLevel(RelatedItem.ID,
                VirtualItemStorage.GetGoodCurrentLevel(RelatedItem.ID) + 1);
        }
    }
}