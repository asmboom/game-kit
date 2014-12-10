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
        public List<string> SubGateIDs;

        public Gate()
        {
            SubGateIDs = new List<string>();
            _subGates = new List<Gate>();
            RefreshSubGateList();
        }

        public bool IsGroup
        {
            get { return Type == GateType.GateListAnd || Type == GateType.GateListOr; }
        }

        public Gate this[int idx]
        {
            get
            {
                return idx >= 0 && idx < SubGateIDs.Count && string.IsNullOrEmpty(SubGateIDs[idx]) ? null :
                    GameKit.Config.GetGateByID(SubGateIDs[idx]);
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

        public List<Gate> GetSubGates(bool refresh)
        {
            if (refresh)
            {
                RefreshSubGateList();
            }
            return _subGates;
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

        private void RefreshSubGateList()
        {
            _subGates.Clear();
            for (int i = 0; i < SubGateIDs.Count; i++)
            {
                _subGates.Add(GameKit.Config.GetGateByID(SubGateIDs[i]));
            }
        }

        private GateDelegate _delegate;
        private List<Gate> _subGates;
    }
}
