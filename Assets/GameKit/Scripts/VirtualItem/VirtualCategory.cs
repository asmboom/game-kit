using System.Collections.Generic;
using UnityEngine;

namespace Beetle23
{
    [System.Serializable]
    public class VirtualCategory : SerializableItem
    {
        [SerializeField]
        public string Name;

        [SerializeField]
        public List<string> ItemIDs;

        public VirtualItem this[int idx]
        {
            get
            {
                return idx >= 0 && idx < ItemIDs.Count && string.IsNullOrEmpty(ItemIDs[idx]) ? null :
                    GameKit.Config.GetVirtualItemByID(ItemIDs[idx]);
            }
        }

        public List<VirtualItem> GetItems(bool refresh)
        {
            if (_items == null)
            {
                _items = new List<VirtualItem>();
                refresh = true;
            }
            if (refresh)
            {
                RefreshItemsList();
            }
            return _items;
        }

        public VirtualCategory()
        {
            ItemIDs = new List<string>();
        }

        private void RefreshItemsList()
        {
            _items.Clear();
            for (int i = 0; i < ItemIDs.Count; i++)
            {
                _items.Add(GameKit.Config.GetVirtualItemByID(ItemIDs[i]));
            }
        }

        private List<VirtualItem> _items;
    }
}
