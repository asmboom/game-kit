using UnityEngine;

public class SingleUseItem : PurchasableItem
{
    public override bool CanPurchaseNow()
    {
        return true;
    }

    protected override void TakeBalance(int amount)
    {
        Storage.SetItemBalance(ID, Storage.GetItemBalance(ID) - amount);
    }

    protected override void GiveBalance(int amount)
    {
        Debug.Log("give amount = " + amount);
        Storage.SetItemBalance(ID, Storage.GetItemBalance(ID) + amount);
    }
}