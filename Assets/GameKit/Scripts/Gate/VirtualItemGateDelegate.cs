using UnityEngine;

namespace Beetle23
{
    public class VirtualItemGateDelegate : GateDelegate
    {
        public VirtualItemGateDelegate(Gate gate)
            : base(gate)
        {
            _requiredBalance = gate.RelatedNumber;
            _virtualItem = gate.RelatedItem as VirtualItem;

            if (_virtualItem != null)
            {
                if (!_context.IsOpened)
                {
                    _virtualItem.OnBalanceChanged += OnItemBalanceChanged;
                }
            }
            else
            {
                Debug.LogError("Virtual item gate [" + gate.Name + "] isn't connected with a virtual item!!!");
            }
        }

        public override bool CanOpenNow
        {
            get
            {
                return _virtualItem != null && _virtualItem.Balance >= _requiredBalance;
            }
        }

        public override void UnregisterEvents()
        {
            if (_virtualItem != null)
            {
                _virtualItem.OnBalanceChanged -= OnItemBalanceChanged;
            }
        }

        public override void RegisterEvents()
        {
            if (_virtualItem != null)
            {
                _virtualItem.OnBalanceChanged += OnItemBalanceChanged;
            }
        }

        private void OnItemBalanceChanged(int oldCount, int newCount)
        {
            if (Gate.AutoSave && _context.CanOpenNow)
            {
                _context.ForceOpen(true);
            }
        }

        private VirtualItem _virtualItem;
        private float _requiredBalance;
    }
}
