using System;
using UnityEngine;

namespace Beetle23
{
    public class LifeTimeItem : PurchasableItem
    {
        [SerializeField]
        public bool IsEquippable;

        public Action OnEquipped = delegate { };
        public Action OnUnEquipped = delegate { };

        public void Give()
        {
            Give(1);
        }

        public void Take()
        {
            Take(1);
        }

        public override bool CanPurchaseNow()
        {
            return Balance < 1;
        }

        protected override void TakeBalance(int amount)
        {
            EconomyStorage.SetItemBalance(ID, 0);
        }

        protected override void GiveBalance(int amount)
        {
            EconomyStorage.SetItemBalance(ID, 1);
        }

        public void Equip()
        {
            if (IsEquippable && EconomyStorage.GetItemBalance(ID) > 0)
            {
                UnequipOtherItemsInCategory();
                EconomyStorage.EquipVirtualGood(ID);
            }
        }

        public void Unequip()
        {
            if (IsEquippable)
            {
                EconomyStorage.UnEquipVirtualGood(ID);
            }
        }

        public bool IsEquipped()
        {
            return IsEquippable && EconomyStorage.IsVertualGoodEquipped(ID);
        }

        private void UnequipOtherItemsInCategory()
        {
            if (Category != null)
            {
                for (int i = 0; i < Category.Items.Count; i++)
                {
                    LifeTimeItem itemInCategory = Category.Items[i] as LifeTimeItem;
                    if (itemInCategory != null &&
                        itemInCategory.IsEquippable &&
                        itemInCategory != this)
                    {
                        itemInCategory.Unequip();
                    }
                }
            }
        }
    }
}