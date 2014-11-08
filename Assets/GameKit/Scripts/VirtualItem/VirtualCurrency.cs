using UnityEngine;

namespace Beetle23
{
    public class VirtualCurrency : VirtualItem
    {
        protected override void TakeBalance(int amount)
        {
            VirtualItemStorage.SetItemBalance(ID, VirtualItemStorage.GetItemBalance(ID) - amount);
        }

        protected override void GiveBalance(int amount)
        {
            VirtualItemStorage.SetItemBalance(ID, VirtualItemStorage.GetItemBalance(ID) + amount);
        }
    }
}
