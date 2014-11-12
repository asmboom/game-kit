using System.Collections.Generic;
using UnityEngine;

namespace Beetle23
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
                case GateType.GateList:
                    return new GateListDelegate(gate);
                default:
                    return null;
            }
        }
    }
}
