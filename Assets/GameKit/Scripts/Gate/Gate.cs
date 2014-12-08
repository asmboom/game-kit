using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Beetle23
{
    [System.Serializable]
    public class Gate : SerializableItem
    {
        public Action OnOpened = delegate { };

        public static bool AutoSave { get; set; }

        [SerializeField]
        public GateType Type;
        [SerializeField]
        public string RelatedItemID;
        [SerializeField]
        public float RelatedNumber;
        [SerializeField]
        public List<Gate> SubGates;

        public bool IsGroup
        {
            get { return Type == GateType.GateListAnd || Type == GateType.GateListOr; }
        }

        public IItem RelatedItem
        {
            get
            {
                return string.IsNullOrEmpty(RelatedItemID) ? null : 
                    GameKit.Config.GetVirtualItemByID(RelatedItemID);
            }
        }

        public bool IsOpened
        {
            get
            {
                return GateStorage.IsOpen(ID);
            }
        }

        public bool CanOpenNow
        {
            get
            {
                if (IsOpened)
                {
                    return false;
                }
                return Delegate.CanOpenNow;
            }
        }

        public  bool TryOpen()
        {
            if (CanOpenNow)
            {
                ForceOpen(true);
                return true;
            }

            return false;
        }

        public void ForceOpen(bool open)
        {
            if (IsOpened == open)
            {
                return;
            }
            GateStorage.SetOpen(ID, open);
            if (open)
            {
                OnOpened();
                Delegate.UnregisterEvents();
            }
            else
            {
                Delegate.RegisterEvents();
            }
        }

        protected GateDelegate Delegate
        {
            get
            {
                if (_delegate == null)
                {
                    _delegate = GateDelegateFactory.Create(this);
                }
                return _delegate;
            }
        }

        private GateDelegate _delegate;
    }
}
