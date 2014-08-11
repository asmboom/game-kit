public interface IEconomyStorage
{
    int GetItemBalance(string itemId);
    void SetItemBalance(string itemId, int balance);

    void EquipVirtualGood(string goodItemId);
    void UnEquipVirtualGood(string goodItemId);
    bool IsVertualGoodEquipped(string goodItemId);

    int GetGoodCurrentLevel(string goodItemId);
    void SetGoodCurrentLevel(string itemId, int level);
}

public class NullableEconomyStorage : IEconomyStorage
{
    public int GetItemBalance(string itemId) { return 0; }
    public void SetItemBalance(string itemId, int balance) { }

    public void EquipVirtualGood(string goodItemId) { }
    public void UnEquipVirtualGood(string goodItemId) { }
    public bool IsVertualGoodEquipped(string goodItemId) { return false; }

    public int GetGoodCurrentLevel(string goodItemId) { return 0; }
    public void SetGoodCurrentLevel(string itemId, int level) { }
}