using System.Collections.Generic;
using UnityEngine;

namespace Codeplay
{
    public static class GateDelegateFactory
    {
        public static GateDelegate Create(Gate gate)
        {
            switch (gate.Type)
            {
                case GateType.VirtualItemGate:
                    return new VirtualItemGateDelegate(gate);
                case GateType.ScoreGate:
                    return new ScoreGateDelegate(gate);
                case GateType.WorldCompletionGate:
                    return new WorldCompletionGateDelegate(gate);
                case GateType.PurchasableGate:
                    return new PurchasableGateDelegate(gate);
                case GateType.GateListAnd:
                case GateType.GateListOr:
                    return new GateListDelegate(gate);
                default:
                    return new NullableGateDelegate(gate);
            }
        }
    }
}
