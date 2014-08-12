using UnityEngine;

public class VirtualCurrency : VirtualItemBase
{
    protected override void TakeBalance(int amount)
    {
        Storage.SetItemBalance(ID, Storage.GetItemBalance(ID) + amount);
    }

    protected override void GiveBalance(int amount)
    {
        Storage.SetItemBalance(ID, Storage.GetItemBalance(ID) - amount);
    }
}
