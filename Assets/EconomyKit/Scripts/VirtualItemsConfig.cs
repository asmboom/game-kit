using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Beetle23
{
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

        public VirtualItem PopulateItemIfNull(string itemID, VirtualItem item)
        {
            if (!string.IsNullOrEmpty(itemID))
            {
#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying)
                {
                    item = null;
                }
#endif
                if (item == null)
                {
                    item = GetItemByID(itemID) as VirtualItem;
                }
            }
            return item;
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
            return _itemIDToCategory.ContainsKey(id) ? _itemIDToCategory[id] : null;
        }

        public List<VirtualItem> GetCategoryItems(VirtualCategory category)
        {
            return _categoryToItems.ContainsKey(category) ? _categoryToItems[category] : null;
        }

        public void UpdateMaps()
        {
            UpdateIdToItemMap();
            UpdateCategoryMaps();
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

            UpdateMaps();
        }

        private void UpdateIdToItemMap()
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
        }

        private void UpdateCategoryMaps()
        {
            _itemIDToCategory = new Dictionary<string, VirtualCategory>();
            _categoryToItems = new Dictionary<VirtualCategory, List<VirtualItem>>();
            for (int i = 0; i < Categories.Count; i++)
            {
                List<VirtualItem> items = new List<VirtualItem>();
                foreach (string itemID in Categories[i].ItemIDs)
                {
                    if (!string.IsNullOrEmpty(itemID))
                    {
                        VirtualItem item = GetItemByID(itemID);
                        if (item != null)
                        {
                            _itemIDToCategory.Add(itemID, Categories[i]);
                            items.Add(item);
                        }
                    }
                }
                _categoryToItems.Add(Categories[i], items);
            }
        }

        private Dictionary<string, VirtualItem> _idToItems;
        private Dictionary<string, VirtualCategory> _itemIDToCategory;
        private Dictionary<VirtualCategory, List<VirtualItem>> _categoryToItems;
    }
}
