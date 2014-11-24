using System.Collections.Generic;
using UnityEngine;

namespace Beetle23
{
    [System.Serializable]
    public class VirtualCategory : SerializableItem
    {
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
            if (refresh)
            {
                RefreshItemsList();
            }
            return _items;
        }

        public VirtualCategory()
        {
            ItemIDs = new List<string>();
            _items = new List<VirtualItem>();
            RefreshItemsList();
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
