using UnityEngine;

namespace Codeplay
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
                Debug.LogError("Virtual item gate [" + gate.ID + "] isn't connected with a virtual item!!!");
            }
        }

        public override IItem GetRelatedItem(string itemID)
        {
            return GameKit.Config.GetVirtualItemByID(itemID);
        }

        public override bool IsOpened
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
            if (_context.IsOpened)
            {
                _context.OnOpened();
            }
        }

        private VirtualItem _virtualItem;
        private float _requiredBalance;
    }
}
