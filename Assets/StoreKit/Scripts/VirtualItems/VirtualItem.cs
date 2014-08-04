using System;
using System.Collections.Generic;
using UnityEngine;

public class VirtualItem : ScriptableObject, IComparable<VirtualItem>
{
    public Action<int, int> OnBalanceChanged = delegate { };
    public Action OnEquipped = delegate { };
    public Action OnUnEquipped = delegate { };

    [SerializeField]
    public VirtualItemType Type;

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
    public string RelatedItemID;

    [SerializeField]
    public int RelatedItemAmount;

    [SerializeField]
    public List<Purchase> PurchaseInfo;

    [SerializeField]
    public List<VirtualItem> Upgrades;

    public bool IsPurchasable { get { return Delegate.IsPurchasableType; } }

    public int Balance { get { return _delegate.GetBalance(this); } }

    public bool HasBalance { get { return _delegate.HasBalance; } }

    public int GainedVirtualCurrencyCountAfterPurchase
    {
        get { return _delegate.GetGainedVirtualCurrencyCountAfterPurchase(this); }
    }

    public bool CanBuyNow { get { return _delegate.CanBuyNow(this); } }

    public bool IsEquippableType { get { return Type == VirtualItemType.EquippableItem; } }

    public bool IsUpgradeType { get { return Type == VirtualItemType.UpgradeItem; } }

    public bool CanUpgrade { get { return NextUpgradeItem != null; } }

    public bool HasUpgrades { get { return Upgrades.Count > 0; } }

    public int MaxLevel { get { return Upgrades.Count; } }

    public int CurrentLevel
    {
        get
        {
            return Upgrades.Count > 0 ?
                Mathf.Clamp(_delegate.GetCurrentLevel(this), 0, MaxLevel) : 0;
        }
    }

    public VirtualItem NextUpgradeItem
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

    public VirtualItem RelatedItem
    {
        get
        {
            if (_relatedItem == null && !string.IsNullOrEmpty(RelatedItemID))
            {
                _relatedItem = StoreKit.Config.GetItemByID(RelatedItemID);
            }
            return _relatedItem;
        }
    }

    public int CompareTo(VirtualItem other)
    {
        return SortIndex.CompareTo(other.SortIndex);
    }

    public void Buy()
    {
        if (IsPurchasable)
        {
            Buy(0);
        }
        else
        {
            Debug.LogError("Trying to buy an item [" + ID + "] that is not purchasable");
        }
    }

    public void Buy(int purchaseIndex)
    {
        purchaseIndex = Mathf.Max(0, purchaseIndex);
        if (purchaseIndex < PurchaseInfo.Count)
        {
            _delegate.Buy(this, PurchaseInfo[purchaseIndex]);
        }
        else
        {
            Debug.LogError("Item [" + ID + "] doesn't have purchase of index [" + purchaseIndex + "]");
        }
    }

    public void Take(int amount)
    {
        int oldBalance = Balance;
        _delegate.Take(this, amount);
        OnBalanceChanged(oldBalance, Balance);
    }

    public void Give(int amount)
    {
        int oldBalance = Balance;
        _delegate.Give(this, amount);
        OnBalanceChanged(oldBalance, Balance);
    }

    public void ResetBalance()
    {
        int oldBalance = Balance;
        _delegate.ResetBalance(this);
        OnBalanceChanged(oldBalance, 0);
    }

    public bool IsEquipped()
    {
        return IsEquippableType &&
            (_delegate as EquippableItemDelegate).IsEquipped(this);
    }

    public void Equip()
    {
        if (IsEquippableType)
        {
            (_delegate as EquippableItemDelegate).Equip(this);
            OnEquipped();
        }
    }

    public void Unequip()
    {
        if (IsEquippableType)
        {
            (_delegate as EquippableItemDelegate).Unequip(this);
            OnUnEquipped();
        }
    }

    public void Upgrade()
    {
        if (CanUpgrade)
        {
            NextUpgradeItem.Buy();
        }
    }

    private VirtualItemDelegate Delegate
    {
        get
        {
            if (_delegate == null)
            {
                _delegate = VirtualItemDelegateFactory.Create(Type);
            }
            return _delegate;
        }
    }

    private void OnEnable()
    {
        _delegate = VirtualItemDelegateFactory.Create(Type);
    }

    private VirtualItemDelegate _delegate;
    private VirtualItem _relatedItem;

    private static IStoreStorage Storage
    {
        get
        {
            if (_storage == null)
            {
                _storage = StoreKit.CreateInventory();
            }
            return _storage;
        }
    }

    private static IStoreStorage _storage;
}