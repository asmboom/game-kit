using System;
using System.Collections.Generic;
using UnityEngine;

namespace Beetle23
{
    [System.Serializable]
    public abstract class VirtualItem : SerializableItem
    {
        public Action<int, int> OnBalanceChanged = delegate { };

        [SerializeField]
        public string Description;
        
        [SerializeField]
        public Sprite Icon;

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

        public VirtualItem()
        {
            Upgrades = new List<UpgradeItem>();
        }

        public void ResetBalance()
        {
            int oldBalance = Balance;
            VirtualItemStorage.SetItemBalance(ID, 0);
            OnBalanceChanged(oldBalance, 0);
        }

        public void Take(int amount)
        {
            int oldBalance = Balance;
            DoTake(amount);
            OnBalanceChanged(oldBalance, Balance);
        }

        public void Give(int amount)
        {
            int oldBalance = Balance;
            DoGive(amount);
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

        protected abstract void DoTake(int amount);
        protected abstract void DoGive(int amount);
    }
}
