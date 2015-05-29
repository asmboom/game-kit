using UnityEngine;

namespace Codeplay
{
    [System.Serializable]
    public class SingleUseItem : PurchasableItem
    {
        public override bool CanPurchaseNow()
        {
            return true;
        }

        protected override void DoTake(int amount)
        {
            VirtualItemStorage.SetItemBalance(ID, VirtualItemStorage.GetItemBalance(ID) - amount);
        }

        protected override void DoGive(int amount)
        {
            VirtualItemStorage.SetItemBalance(ID, VirtualItemStorage.GetItemBalance(ID) + amount);
        }
    }
}