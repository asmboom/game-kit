using System.Collections.Generic;
using UnityEngine;

namespace Beetle23
{
    [System.Serializable]
    public class VirtualCategory
    {
        public string ID;
        public List<VirtualItem> Items;

        public VirtualCategory()
        {
            Items = new List<VirtualItem>();
        }
    }
}
