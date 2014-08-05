using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class VirtualItemsConfig : ScriptableObject
{
    [SerializeField]
    public List<VirtualItem> Items;

    [SerializeField]
    public List<VirtualCategory> Categories;

    public ReadOnlyCollection<VirtualItem> VirtualCurrencies
    {
        get { return _typeToItemList[VirtualItemType.VirtualCurrency].AsReadOnly(); }
    }

    public ReadOnlyCollection<VirtualItem> VirtualCurrencyPacks
    {
        get { return _typeToItemList[VirtualItemType.VirtualCurrencyPack].AsReadOnly(); }
    }

    public ReadOnlyCollection<VirtualItem> Consumables
    {
        get { return _typeToItemList[VirtualItemType.ConsumableItem].AsReadOnly(); }
    }

    public ReadOnlyCollection<VirtualItem> ConsumablePacks
    {
        get { return _typeToItemList[VirtualItemType.ConsumablePack].AsReadOnly(); }
    }

    public ReadOnlyCollection<VirtualItem> NonConsumables
    {
        get { return _typeToItemList[VirtualItemType.NonConsumableItem].AsReadOnly(); }
    }

    public ReadOnlyCollection<VirtualItem> EquippableItems
    {
        get { return _typeToItemList[VirtualItemType.EquippableItem].AsReadOnly(); }
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

    private void OnEnable()
    {
        if (Items == null)
        {
            Items = new List<VirtualItem>();
        }
        if (Categories == null)
        {
            Categories = new List<VirtualCategory>();
        }

        _idToItems = new Dictionary<string, VirtualItem>();
        for (int i = 0; i < Items.Count; i++)
        {
            _idToItems.Add(Items[i].ID, Items[i]);
        }

        _typeToItemList = new Dictionary<VirtualItemType, List<VirtualItem>>();
        foreach (string itemTypeStr in System.Enum.GetNames(typeof(VirtualItemType)))
        {
            VirtualItemType type = (VirtualItemType)System.Enum.Parse(typeof(VirtualItemType), itemTypeStr);
            _typeToItemList.Add(type, new List<VirtualItem>());
        }

        for (int i = 0; i < Items.Count; i++)
        {
            _typeToItemList[Items[i].Type].Add(Items[i]);
        }
    }

    private Dictionary<string, VirtualItem> _idToItems;
    private Dictionary<VirtualItemType, List<VirtualItem>> _typeToItemList;
}
