using System;
using System.Collections.Generic;
using UnityEngine;

namespace Beetle23
{
    [System.Serializable]
    public abstract class VirtualItem : Entity
    {
        public Action<int, int> OnBalanceChanged = delegate { };

        public Sprite Icon;
        public List<UpgradeItem> Upgrades;
        public ScriptableObject Extend;

        public int Balance { get { return EconomyStorage.GetItemBalance(ID); } }

        public VirtualCategory Category
        {
            get
            {
                return string.IsNullOrEmpty(ID) ? null : EconomyKit.Config.GetItemCategory(ID);
            }
        }

        public VirtualItem()
        {
            Upgrades = new List<UpgradeItem>();
        }

        public void ResetBalance()
        {
            int oldBalance = Balance;
            EconomyStorage.SetItemBalance(ID, 0);
            OnBalanceChanged(oldBalance, 0);
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
                    Mathf.Clamp(EconomyStorage.GetGoodCurrentLevel(ID), 0, MaxLevel) : 0;
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
