using System.Collections.Generic;
using UnityEngine;

namespace Codeplay
{
    public class GateListDelegate : GateDelegate
    {
        public GateListDelegate(Gate gate)
            : base(gate)
        {
            if (!_context.IsOpened)
            {
				foreach (var subGateID in _context.SubGatesID)
                {
					GameKit.Config.GetSubGateByID(subGateID).OnOpened += OnGateOpened;
                }
            }
        }

        public override IItem GetRelatedItem(string itemID)
        {
            return null;
        }

        public override bool IsOpened
        {
            get
            {
                if (_context.Type == GateType.GateListAnd)
                {
					foreach (string subGateID in _context.SubGatesID)
                    {
						if (!GameKit.Config.GetSubGateByID(subGateID).IsOpened)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
					foreach (string subGateID in _context.SubGatesID)
                    {
						if (GameKit.Config.GetSubGateByID(subGateID).IsOpened)
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
			foreach (var subGateID in _context.SubGatesID)
            {
				GameKit.Config.GetSubGateByID(subGateID).OnOpened += OnGateOpened;
            }
        }

        public override void UnregisterEvents()
        {
			foreach (var subGateID in _context.SubGatesID)
            {
				GameKit.Config.GetSubGateByID(subGateID).OnOpened -= OnGateOpened;
            }
        }

        private void OnGateOpened()
        {
            if (_context.IsOpened)
            {
                _context.OnOpened();
            }
        }
    }
}
