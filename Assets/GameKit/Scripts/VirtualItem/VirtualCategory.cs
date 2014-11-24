using System.Collections.Generic;
using UnityEngine;

namespace Beetle23
{
    [System.Serializable]
    public class VirtualCategory : SerializableItem
    {
        [SerializeField]
        public List<VirtualItem> Items;

        public VirtualCategory()
        {
            Items = new List<VirtualItem>();
        }
    }
}
