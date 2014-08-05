public interface IEconomyStorage
{
    int GetItemBalance(string itemId);
    void SetItemBalance(string itemId, int balance);
    void AddItemBalance(string itemId, int amount);
    void RemoveItemBalance(string itemId, int amount);

    void EquipVirtualGood(string goodItemId);
    void UnEquipVirtualGood(string goodItemId);
    bool IsVertualGoodEquipped(string goodItemId);

    int GetGoodCurrentLevel(string goodItemId);
    void UpgradeGood(string goodItemId);
    void DowngradeGood(string goodItemId);
    void RemoveGoodUpgrades(string goodItemId);
}

public class NullableEconomyStorage : IEconomyStorage
{
    public int GetItemBalance(string itemId) { return 0; }
    public void SetItemBalance(string itemId, int balance) { }
    public void AddItemBalance(string itemId, int amount) { }
    public void RemoveItemBalance(string itemId, int amount) { }
    public void EquipVirtualGood(string goodItemId) { }
    public void UnEquipVirtualGood(string goodItemId) { }
    public bool IsVertualGoodEquipped(string goodItemId) { return false; }
    public int GetGoodCurrentLevel(string goodItemId) { return 0; }
    public void UpgradeGood(string goodItemId) { }
    public void DowngradeGood(string goodItemId) { }
    public void RemoveGoodUpgrades(string goodItemId) { }
}