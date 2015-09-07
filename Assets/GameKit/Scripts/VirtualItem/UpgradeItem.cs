using UnityEngine;

namespace Codeplay
{
    [System.Serializable]
    public class UpgradeItem : PurchasableItem
    {
		[SerializeField]
		public string RelatedItemID;

		public VirtualItem RelatedItem
		{
			get
			{
				return GameKit.Config.GetVirtualItemByID(RelatedItemID);
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
			VirtualItemStorage.SetGoodCurrentLevel(RelatedItemID,
                VirtualItemStorage.GetGoodCurrentLevel(RelatedItemID) - 1);
        }

        protected override void DoGive(int amount)
        {
			VirtualItemStorage.SetGoodCurrentLevel(RelatedItemID,
                VirtualItemStorage.GetGoodCurrentLevel(RelatedItemID) + 1);
        }
    }
}