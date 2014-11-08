using UnityEngine;

namespace Beetle23
{
    public class SingleUseItem : PurchasableItem
    {
        public override bool CanPurchaseNow()
        {
            return true;
        }

        protected override void TakeBalance(int amount)
        {
            EconomyStorage.SetItemBalance(ID, EconomyStorage.GetItemBalance(ID) - amount);
        }

        protected override void GiveBalance(int amount)
        {
            EconomyStorage.SetItemBalance(ID, EconomyStorage.GetItemBalance(ID) + amount);
        }
    }
}