using System.Collections.Generic;
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
                List<Gate> subGates = _context.GetSubGates(false);
                foreach (var aGate in subGates)
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
                List<Gate> subGates = _context.GetSubGates(false);
                if (_context.Type == GateType.GateListAnd)
                {
                    foreach (Gate gate in subGates)
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
                    foreach (Gate gate in subGates)
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
            List<Gate> subGates = _context.GetSubGates(false);
            foreach (var gate in subGates)
            {
                gate.OnOpened += OnGateOpened;
            }
        }

        public override void UnregisterEvents()
        {
            List<Gate> subGates = _context.GetSubGates(false);
            foreach (var gate in subGates)
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
