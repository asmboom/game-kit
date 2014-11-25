using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Beetle23
{
    public class GameKitConfig : ScriptableObject
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
        public List<VirtualCategory> Categories;

        [SerializeField]
        public World RootWorld;

        public IEnumerable<VirtualItem> VirtualItems
        {
            get
            {
                return _idToVirtualItems.Values;
            }
        }

        public int VirtualItemsCount
        {
            get { return _idToVirtualItems.Count; }
        }

        public bool TryGetVirtualItemByID(string id, out VirtualItem item)
        {
            return _idToVirtualItems.TryGetValue(id, out item);
        }

        public VirtualItem GetVirtualItemByID(string id)
        {
            if (_idToVirtualItems.ContainsKey(id))
            {
                return _idToVirtualItems[id];
            }
            else
            {
                return null;
            }
        }

        public VirtualCategory GetCategoryByID(string id)
        {
            foreach (var category in Categories)
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
            _idToVirtualItems = new Dictionary<string, VirtualItem>();
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
        }

        public void RemoveNullRefs()
        {
            for (int i = 0; i < VirtualCurrencies.Count; i++)
            {
                if (VirtualCurrencies[i] == null)
                {
                    VirtualCurrencies.RemoveAt(i);
                }
            }
            for (int i = 0; i < SingleUseItems.Count; i++)
            {
                if (SingleUseItems[i] == null)
                {
                    SingleUseItems.RemoveAt(i);
                }
            }
            for (int i = 0; i < LifeTimeItems.Count; i++)
            {
                if (LifeTimeItems[i] == null)
                {
                    LifeTimeItems.RemoveAt(i);
                }
            }
            for (int i = 0; i < ItemPacks.Count; i++)
            {
                if (ItemPacks[i] == null)
                {
                    ItemPacks.RemoveAt(i);
                }
            }
            for (int i = 0; i < Categories.Count; i++)
            {
                if (Categories[i] == null)
                {
                    Categories.RemoveAt(i);
                }
            }
        }

        public void UpdateIdToCategoryMap()
        {
            _idToCategory = new Dictionary<string, VirtualCategory>();
            for (int i = 0; i < Categories.Count; i++)
            {
                foreach (var itemID in Categories[i].ItemIDs)
                {
                    if (itemID != null && GetVirtualItemByID(itemID) != null)
                    {
                        _idToCategory.Add(itemID, Categories[i]);
                    }
                }
            }
        }

        private void TryAddToIdItemMap(string id, VirtualItem item)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!_idToVirtualItems.ContainsKey(id))
                {
                    _idToVirtualItems.Add(id, item);
                }
                else
                {
                    Debug.LogWarning("Found duplicated id " + id + " for item."); ;
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
            if (Categories == null)
            {
                Categories = new List<VirtualCategory>();
            }

            UpdateIdToItemMap();
            UpdateIdToCategoryMap();
        }

        private Dictionary<string, VirtualItem> _idToVirtualItems;
        private Dictionary<string, VirtualCategory> _idToCategory;
    }
}
