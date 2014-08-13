﻿using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class VirtualItem : ScriptableObject, IComparable
{
    public Action<int, int> OnBalanceChanged = delegate { };

    [SerializeField]
    public string Name;

    [SerializeField]
    public string Description;

    [SerializeField]
    public string ID;

    [SerializeField]
    public int SortIndex;

    [SerializeField]
    public VirtualCategory Category;

    [SerializeField]
    public List<UpgradeItem> Upgrades;

    public int Balance { get { return Storage.GetItemBalance(ID); } }

    public void ResetBalance()
    {
        int oldBalance = Balance;
        Storage.SetItemBalance(ID, 0);
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
                Mathf.Clamp(Storage.GetGoodCurrentLevel(ID), 0, MaxLevel) : 0;
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

    protected virtual void OnEnable()
    {
        if (Upgrades == null)
        {
            Upgrades = new List<UpgradeItem>();
        }
    }

    protected abstract void TakeBalance(int amount);
    protected abstract void GiveBalance(int amount);

    protected static IEconomyStorage Storage
    {
        get
        {
            if (_storage == null)
            {
                _storage = EconomyKit.CreateStorage();
            }
            return _storage;
        }
    }

    protected static IEconomyStorage _storage;
}