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

    public void UpdateIdToItemMap()
    {
        _idToItems = new Dictionary<string, VirtualItem>();
        for (int i = 0; i < VirtualCurrencies.Count; i++)
        {
            _idToItems.Add(VirtualCurrencies[i].ID, VirtualCurrencies[i]);
        }
        for (int i = 0; i < SingleUseItems.Count; i++)
        {
            _idToItems.Add(SingleUseItems[i].ID, SingleUseItems[i]);
        }
        for (int i = 0; i < LifeTimeItems.Count; i++)
        {
            _idToItems.Add(LifeTimeItems[i].ID, LifeTimeItems[i]);
        }
        for (int i = 0; i < ItemPacks.Count; i++)
        {
            _idToItems.Add(ItemPacks[i].ID, ItemPacks[i]);
        }
        for (int i = 0; i < UpgradeItems.Count; i++)
        {
            _idToItems.Add(UpgradeItems[i].ID, UpgradeItems[i]);
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
    }

    private Dictionary<string, VirtualItem> _idToItems;
}
