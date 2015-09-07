using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Codeplay
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
		public List<string> SubGatesID;
		[NonSerialized]
        public List<Gate> SubGates;

        public Gate()
        {
			SubGatesID = new List<string>();
            SubGates = new List<Gate>();
        }

		public void RefreshSubGates()
		{
			SubGates.Clear();
			for (int i = 0; i < SubGatesID.Count; i++)
			{
				SubGates.Add(GameKit.Config.GetSubGateByID(SubGatesID[i]));
			}
		}

        public bool IsGroup
        {
            get { return Type == GateType.GateListAnd || Type == GateType.GateListOr; }
        }

        public Gate this[int idx]
        {
            get
            {
				return idx >= 0 && idx < SubGatesID.Count ? GameKit.Config.GetSubGateByID(SubGatesID[idx]) : null;
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
