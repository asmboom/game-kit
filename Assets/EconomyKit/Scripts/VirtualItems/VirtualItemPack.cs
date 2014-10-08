using System.Collections.Generic;
using UnityEngine;

namespace Beetle23
{
    [System.Serializable]
    public class PackElement
    {
        public VirtualItem Item;
        public int Amount;

        public void Give(int amount)
        {
            Item.Give(amount * Amount);
        }

        public void Take(int amount)
        {
            Item.Take(amount * Amount);
        }

        public override string ToString()
        {
            return Item != null ? string.Format("{0}x{1}", Item.Name, Amount) : "None";
        }
    }

    public class VirtualItemPack : PurchasableItem
    {
        [SerializeField]
        public List<PackElement> PackElements;

        public override bool CanPurchaseNow()
        {
            return true;
        }

        protected override void TakeBalance(int amount)
        {
            for (int i = 0; i < PackElements.Count; i++)
            {
                PackElements[i].Take(amount);
            }
        }

        protected override void GiveBalance(int amount)
        {
            for (int i = 0; i < PackElements.Count; i++)
            {
                PackElements[i].Give(amount);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (PackElements == null)
            {
                PackElements = new List<PackElement>();
            }
        }

        public override string ToString()
        {
            if (PackElements.Count > 0)
            {
                string final = string.Empty;
                for (int i = 0; i < PackElements.Count; i++)
                {
                    final += PackElements[i].ToString();
                    if (i < PackElements.Count - 1)
                    {
                        final += ",";
                    }
                }
                return final;
            }
            else
            {
                return EmptyString;
            }
        }

        private const string EmptyString = "Empty";
    }
}