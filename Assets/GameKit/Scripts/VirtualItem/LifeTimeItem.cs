using System;
using UnityEngine;

namespace Beetle23
{
    [System.Serializable]
    public class LifeTimeItem : PurchasableItem
    {
        [SerializeField]
        public bool IsEquippable;

        public Action OnEquipped = delegate { };
        public Action OnUnequipped = delegate { };

        public bool Owned
        {
            get
            {
                return Balance >= 1;
            }
        }

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

        protected override void DoTake(int amount)
        {
            VirtualItemStorage.SetItemBalance(ID, 0);
        }

        protected override void DoGive(int amount)
        {
            VirtualItemStorage.SetItemBalance(ID, 1);
        }

        public void Equip()
        {
            if (IsEquippable && VirtualItemStorage.GetItemBalance(ID) > 0)
            {
                UnequipOtherItemsInCategory();
                VirtualItemStorage.EquipVirtualGood(ID);
                OnEquipped();
            }
        }

        public void Unequip()
        {
            if (IsEquippable)
            {
                VirtualItemStorage.UnEquipVirtualGood(ID);
                OnUnequipped();
            }
        }

        public bool IsEquipped()
        {
            return IsEquippable && VirtualItemStorage.IsVertualGoodEquipped(ID);
        }

        private void UnequipOtherItemsInCategory()
        {
            if (Category != null)
            {
                System.Collections.Generic.List<VirtualItem> items = Category.GetItems(false);
                for (int i = 0; i < items.Count; i++)
                {
                    LifeTimeItem itemInCategory = items[i] as LifeTimeItem;
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