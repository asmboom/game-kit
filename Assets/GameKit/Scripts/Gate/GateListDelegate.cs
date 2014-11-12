using UnityEngine;

namespace Beetle23
{
    public class GateListDelegate : GateDelegate
    {
        public GateListDelegate(Gate gate)
            : base(gate)
        {
            _gateList = gate as GatesList;

            if (!_gateList.IsOpened)
            {
                foreach (var aGate in _gateList.Gates)
                {
                    aGate.OnOpened += OnGateOpened;
                }
            }
        }

        public override bool CanOpenNow
        {
            get
            {
                if (_gateList.ListType == GatesList.GateListType.And)
                {
                    foreach (Gate gate in _gateList.Gates)
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
                    foreach (Gate gate in _gateList.Gates)
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

        public override void HandleOnClose()
        {
            foreach (var gate in _gateList.Gates)
            {
                gate.OnOpened += OnGateOpened;
            }
        }

        public override void HandleOnOpen()
        {
            foreach (var gate in _gateList.Gates)
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

        private GatesList _gateList;
    }
}
