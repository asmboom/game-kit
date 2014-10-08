using System.Collections.Generic;
using UnityEngine;

namespace Beetle23
{
    [System.Serializable]
    public class VirtualCategory
    {
        public string ID;
        public List<string> ItemIDs;

        public List<VirtualItem> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = EconomyKit.Config.GetCategoryItems(this);
                }
                return _items;
            }
        }

        public VirtualCategory()
        {
            ItemIDs = new List<string>();
        }

        private List<VirtualItem> _items;
    }
}
