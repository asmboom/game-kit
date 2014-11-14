using UnityEngine;
using System.Collections;
using System;

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
        public ScriptableItem RelatedItem;
        [SerializeField]
        public float RelatedNumber;

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

        public T GetRelatedItem<T>() where T : ScriptableItem
        {
            return RelatedItem as T;
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
