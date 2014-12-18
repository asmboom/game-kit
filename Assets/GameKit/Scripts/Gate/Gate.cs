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

        [SerializeField]
        public GateType Type;
        [SerializeField]
        public string RelatedItemID;
        [SerializeField]
        public float RelatedNumber;
        [SerializeField]
        public List<Gate> SubGates;

        public Gate()
        {
            SubGates = new List<Gate>();
        }

        public bool IsGroup
        {
            get { return Type == GateType.GateListAnd || Type == GateType.GateListOr; }
        }

        public Gate this[int idx]
        {
            get
            {
                return idx >= 0 && idx < SubGates.Count ? SubGates[idx] : null;
            }
        }

        public IItem RelatedItem
        {
            get
            {
                return string.IsNullOrEmpty(RelatedItemID) ? null : GameKit.Config.GetScoreByID(RelatedItemID);
            }
        }

        public bool IsOpened
        {
            get
            {
                return Delegate.IsOpened;
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
