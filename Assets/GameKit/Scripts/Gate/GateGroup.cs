using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Beetle23
{
    [System.Serializable]
    public class GateGroup : Gate
    {
        public enum GateGroupType
        {
            And,
            Or
        }

        [SerializeField]
        public List<Gate> Gates;
        [SerializeField]
        public GateGroupType GroupType;

        public GateGroup()
        {
            if (Type != GateType.GateList)
            {
                Type = GateType.GateList;
                _delegate = GateDelegateFactory.Create(this);
            }
            Gates = new List<Gate>();
        }

        public Gate this[string id]
        {
            get
            {
                foreach (Gate gate in Gates)
                {
                    if (gate.ID == id)
                    {
                        return gate;
                    }
                }

                return null;
            }
        }

        public Gate this[int idx]
        {
            get { return Gates[idx]; }
            set { Gates[idx] = value; }
        }
    }
}