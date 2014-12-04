using UnityEngine;

namespace Beetle23
{
    [System.Serializable]
    public class UpgradeItem : PurchasableItem
    {
        public VirtualItem RelatedItem
        {
            get
            {
                if (_relatedItem == null)
                {
                    _relatedItem = GameKit.Config.FindVirtualItemThatUpgradeBelongsTo(this);
                }
                return _relatedItem;
            }
        }

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

        private VirtualItem _relatedItem;
    }
}