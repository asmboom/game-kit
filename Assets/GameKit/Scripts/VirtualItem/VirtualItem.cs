using System;
using System.Collections.Generic;
using UnityEngine;

namespace Beetle23
{
    public abstract class VirtualItem : ScriptableItem, IComparable
    {
        public Action<int, int> OnBalanceChanged = delegate { };
        
        [SerializeField]
        public Sprite Icon;

        [SerializeField]
        [HideInInspector]
        public int SortIndex;

        [SerializeField]
        public List<UpgradeItem> Upgrades;

        [SerializeField]
        public ScriptableObject Extend;

        public int Balance { get { return VirtualItemStorage.GetItemBalance(ID); } }

        public VirtualCategory Category
        {
            get
            {
                return GameKit.Config.GetItemCategory(ID);
            }
        }

        public void ResetBalance()
        {
            int oldBalance = Balance;
            VirtualItemStorage.SetItemBalance(ID, 0);
            OnBalanceChanged(oldBalance, 0);
        }

        public int CompareTo(object obj)
        {
            VirtualItem otherItem = obj as VirtualItem;
            return otherItem != null ?
                SortIndex.CompareTo(otherItem.SortIndex) : 0;
        }

        public int CompareTo(VirtualItem other)
        {
            return SortIndex.CompareTo(other.SortIndex);
        }

        public void Take(int amount)
        {
            int oldBalance = Balance;
            TakeBalance(amount);
            OnBalanceChanged(oldBalance, Balance);
        }

        public void Give(int amount)
        {
            int oldBalance = Balance;
            GiveBalance(amount);
            OnBalanceChanged(oldBalance, Balance);
        }

        public bool HasUpgrades { get { return Upgrades.Count > 0; } }

        public bool CanUpgrade { get { return Upgrades.Count > 0 && NextUpgradeItem != null; } }

        public int MaxLevel { get { return Upgrades.Count; } }

        public int CurrentLevel
        {
            get
            {
                return Upgrades.Count > 0 ?
                    Mathf.Clamp(VirtualItemStorage.GetGoodCurrentLevel(ID), 0, MaxLevel) : 0;
            }
        }

        public UpgradeItem NextUpgradeItem
        {
            get
            {
                if (Upgrades.Count > 0)
                {
                    int currentLevel = CurrentLevel;
                    if (currentLevel < MaxLevel)
                    {
                        return Upgrades[currentLevel];
                    }
                }
                return null;
            }
        }

        public void Upgrade()
        {
            if (CanUpgrade)
            {
                NextUpgradeItem.Purchase();
            }
        }

        public T GetExtend<T>() where T : ScriptableObject
        {
            return Extend as T;
        }

        protected virtual void OnEnable()
        {
            if (Upgrades == null)
            {
                Upgrades = new List<UpgradeItem>();
            }
        }

        protected abstract void TakeBalance(int amount);
        protected abstract void GiveBalance(int amount);
    }
}
