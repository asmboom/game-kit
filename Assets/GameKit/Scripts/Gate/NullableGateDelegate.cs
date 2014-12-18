using UnityEngine;
using System;
using System.Collections;

namespace Beetle23
{
    public class NullableGateDelegate : GateDelegate
    {
        public NullableGateDelegate(Gate gate)
            : base(gate)
        { }

        public override IItem GetRelatedItem(string itemID) { return null; }
        public override bool IsOpened { get { return true; } }
        public override void UnregisterEvents() { }
        public override void RegisterEvents() { }
    }
}