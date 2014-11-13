using UnityEngine;

namespace Beetle23
{
    public class GateGroupDelegate : GateDelegate
    {
        public GateGroupDelegate(Gate gate)
            : base(gate)
        {
            _gateGroup = gate as GateGroup;

            if (!_gateGroup.IsOpened)
            {
                foreach (var aGate in _gateGroup.Gates)
                {
                    aGate.OnOpened += OnGateOpened;
                }
            }
        }

        public override bool CanOpenNow
        {
            get
            {
                if (_gateGroup.GroupType == GateGroup.GateGroupType.And)
                {
                    foreach (Gate gate in _gateGroup.Gates)
                    {
                        if (!gate.IsOpened)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    foreach (Gate gate in _gateGroup.Gates)
                    {
                        if (gate.IsOpened)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
        }

        public override void RegisterEvents()
        {
            foreach (var gate in _gateGroup.Gates)
            {
                gate.OnOpened += OnGateOpened;
            }
        }

        public override void UnregisterEvents()
        {
            foreach (var gate in _gateGroup.Gates)
            {
                gate.OnOpened -= OnGateOpened;
            }
        }

        private void OnGateOpened()
        {
            if (_context.CanOpenNow)
            {
                _context.ForceOpen(true);
            }
        }

        private GateGroup _gateGroup;
    }
}
