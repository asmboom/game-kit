using UnityEngine;

public class LifeTimeItem : PurchasableItem
{
    protected override bool CanPurchaseNow()
    {
        return Balance < 1;
    }

    protected override void TakeBalance(int amount)
    {
        Storage.SetItemBalance(ID, 0);
    }

    protected override void GiveBalance(int amount)
    {
        Storage.SetItemBalance(ID, 1);
    }
}