using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class VirtualItemsConfig : ScriptableObject
{
    [SerializeField]
    public List<VirtualCurrency> VirtualCurrencies;

	[SerializeField]
	public List<SingleUseItem> SingleUseItems;

	[SerializeField]
	public List<LifeTimeItem> LifeTimeItems;

    [SerializeField]
    public List<VirtualItemPack> ItemPacks;

    [SerializeField]
    public List<UpgradeItem> UpgradeItems;

    [SerializeField]
    public List<VirtualCategory> Categories;

    public IEnumerable<VirtualItem> Items
    {
        get
        {
            return _idToItems.Values;
        }
    }

    public int ItemsCount
    {
        get { return _idToItems.Count; }
    }

    public bool TryGetItemByID(string id, out VirtualItem item)
    {
        return _idToItems.TryGetValue(id, out item);
    }

    public VirtualItem GetItemByID(string id)
    {
        if (_idToItems.ContainsKey(id))
        {
            return _idToItems[id];
        }
        else
        {
            return null;
        }
    }

    public VirtualCategory GetCategoryByID(string id)
    {
        foreach( var category in Categories )
        {
            if (category.ID.Equals(id))
            {
                return category;
            }
        }
        return null;
    }

    public VirtualCategory GetItemCategory(string id)
    {
        return _idToCategory.ContainsKey(id) ? _idToCategory[id] : null;
    }

    public void UpdateIdToItemMap()
    {
        _idToItems = new Dictionary<string, VirtualItem>();
        for (int i = 0; i < VirtualCurrencies.Count; i++)
        {
            TryAddToIdItemMap(VirtualCurrencies[i].ID, VirtualCurrencies[i]);
        }
        for (int i = 0; i < SingleUseItems.Count; i++)
        {
            TryAddToIdItemMap(SingleUseItems[i].ID, SingleUseItems[i]);
        }
        for (int i = 0; i < LifeTimeItems.Count; i++)
        {
            TryAddToIdItemMap(LifeTimeItems[i].ID, LifeTimeItems[i]);
        }
        for (int i = 0; i < ItemPacks.Count; i++)
        {
            TryAddToIdItemMap(ItemPacks[i].ID, ItemPacks[i]);
        }
        for (int i = 0; i < UpgradeItems.Count; i++)
        {
            TryAddToIdItemMap(UpgradeItems[i].ID, UpgradeItems[i]);
        }
    }

    public void UpdateIdToCategoryMap()
    {
        _idToCategory = new Dictionary<string, VirtualCategory>();
        for (int i = 0; i < Categories.Count; i++)
        {
            foreach (var item in Categories[i].Items)
            {
                if (item != null)
                {
                    _idToCategory.Add(item.ID, Categories[i]);
                }
            }
        }
    }

    private void TryAddToIdItemMap(string id, VirtualItem item)
    {
        if (!string.IsNullOrEmpty(id))
        {
            if (!_idToItems.ContainsKey(id))
            {
                _idToItems.Add(id, item);
            }
            else
            {
                Debug.LogWarning("Found duplicated id " + id + " for item.");;
            }
        }
    }

    private void OnEnable()
    {
        if (VirtualCurrencies == null)
        {
            VirtualCurrencies = new List<VirtualCurrency>();
        }
		if (SingleUseItems == null)
		{
			SingleUseItems = new List<SingleUseItem>();
		}
		if (LifeTimeItems == null)
		{
			LifeTimeItems = new List<LifeTimeItem>();
		}
        if (ItemPacks == null)
        {
            ItemPacks = new List<VirtualItemPack>();
        }
        if (UpgradeItems == null)
        {
            UpgradeItems = new List<UpgradeItem>();
        }
        if (Categories == null)
        {
            Categories = new List<VirtualCategory>();
        }

        UpdateIdToItemMap();
        UpdateIdToCategoryMap();
    }

    private Dictionary<string, VirtualItem> _idToItems;
    private Dictionary<string, VirtualCategory> _idToCategory;
}
