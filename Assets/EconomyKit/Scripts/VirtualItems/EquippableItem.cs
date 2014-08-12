using System;
using UnityEngine;

public class EquippableItem : LifeTimeItem
{
    public Action OnEquipped = delegate { };
    public Action OnUnEquipped = delegate { };

    public void Equip()
    {
        if (Storage.GetItemBalance(ID) > 0)
        {
            UnequipOtherItemsInCategory();
            Storage.EquipVirtualGood(ID);
        }
    }

    public void Unequip()
    {
        Storage.UnEquipVirtualGood(ID);
    }

    public bool IsEquipped()
    {
        return Storage.IsVertualGoodEquipped(ID);
    }

    private void UnequipOtherItemsInCategory()
    {
        if (Category != null)
        {
            for (int i = 0; i < Category.Items.Count; i++)
            {
                VirtualItem itemInCategory = Category.Items[i];
                if (itemInCategory != this)
                {
                    itemInCategory.Unequip();
                }
            }
        }
    }
}