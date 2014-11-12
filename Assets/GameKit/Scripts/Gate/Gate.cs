using UnityEngine;
using System.Collections;
using System;

namespace Beetle23
{
    [System.Serializable]
    public class Gate : Item
    {
        public Action OnOpened = delegate { };

        public GateType Type;
        public ScriptableItem RelatedItem;
        public float RelatedNumber;

        public Gate()
        {
            _delegate = GateDelegateFactory.Create(this);
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
                return _delegate.CanOpenNow;
            }
        }

        public bool TryOpen()
        {
            if (GateStorage.IsOpen(ID))
            {
                return true;
            }
            return _delegate.TryOpen();
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
                _delegate.HandleOnOpen();
            }
            else
            {
                _delegate.HandleOnClose();
            }
        }

        public T GetRelatedItem<T>() where T : ScriptableItem
        {
            return RelatedItem as T;
        }

        protected GateDelegate _delegate;
    }
}
