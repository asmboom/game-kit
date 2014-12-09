using UnityEngine;

namespace Beetle23
{
    public class GateListDelegate : GateDelegate
    {
        public GateListDelegate(Gate gate)
            : base(gate)
        {
            if (!_context.IsOpened)
            {
                foreach (var aGate in _context.SubGates)
                {
                    aGate.OnOpened += OnGateOpened;
                }
            }
        }

        public override IItem GetRelatedItem(string itemID)
        {
            return null;
        }

        public override bool CanOpenNow
        {
            get
            {
                if (_context.Type == GateType.GateListAnd)
                {
                    foreach (Gate gate in _context.SubGates)
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
                    foreach (Gate gate in _context.SubGates)
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
            foreach (var gate in _context.SubGates)
            {
                gate.OnOpened += OnGateOpened;
            }
        }

        public override void UnregisterEvents()
        {
            foreach (var gate in _context.SubGates)
            {
                gate.OnOpened -= OnGateOpened;
            }
        }

        private void OnGateOpened()
        {
            if (Gate.AutoSave && _context.CanOpenNow)
            {
                _context.ForceOpen(true);
            }
        }
    }
}
