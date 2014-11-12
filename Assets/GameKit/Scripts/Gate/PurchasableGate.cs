using UnityEngine;
using System.Collections;

namespace Beetle23
{
    public class PurchasableGate : GateDelegate
    {
        public PurchasableGate(Gate gate)
            : base(gate)
        {
            _lifetimeItem = gate.GetRelatedItem<LifeTimeItem>();

            if (_lifetimeItem != null)
            {
                if (!_context.IsOpened)
                {
                    _lifetimeItem.OnPurchased += OnPurchasedItem;
                }
            }
            else
            {
                Debug.LogError("Purchasable gate [" + gate.Name + "] isn't connected with a purchasable lifetime item!!!");
            }
        }

        public override bool CanOpenNow
        {
            get
            {
                return _lifetimeItem != null && _lifetimeItem.Owned;
            }
        }

        public override void HandleOnOpen()
        {
            _lifetimeItem.OnPurchased -= OnPurchasedItem;
        }

        public override void HandleOnClose()
        {
            _lifetimeItem.OnPurchased += OnPurchasedItem;
        }

        private void OnPurchasedItem()
        {
            _context.ForceOpen(true);
        }

        private LifeTimeItem _lifetimeItem;
    }
}

