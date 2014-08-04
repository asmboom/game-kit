public class EquippableItemDelegate : NonConsumableDelegate
{
    public void Equip(VirtualItem item)
    {
        if (Storage.GetItemBalance(item.ID) > 0)
        {
            UnequipOtherItemsInCategory(item);
            Storage.EquipVirtualGood(item.ID);
        }
    }

    public void Unequip(VirtualItem item)
    {
        Storage.UnEquipVirtualGood(item.ID);
    }

    public bool IsEquipped(VirtualItem item)
    {
        return Storage.IsVertualGoodEquipped(item.ID);
    }

    private void UnequipOtherItemsInCategory(VirtualItem item)
    {
        if (item.Category != null)
        {
            for (int i = 0; i < item.Category.Items.Count; i++)
            {
                VirtualItem itemInCategory = item.Category.Items[i];
                if (itemInCategory != item)
                {
                    itemInCategory.Unequip();
                }
            }
        }
    }
}