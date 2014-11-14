using UnityEngine;
using System.Collections;

namespace Beetle23
{
    public class PurchasableGateDelegate : GateDelegate
    {
        public PurchasableGateDelegate(Gate gate)
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

        public override void UnregisterEvents()
        {
            _lifetimeItem.OnPurchased -= OnPurchasedItem;
        }

        public override void RegisterEvents()
        {
            _lifetimeItem.OnPurchased += OnPurchasedItem;
        }

        private void OnPurchasedItem()
        {
            if (Gate.AutoSave)
            {
                _context.ForceOpen(true);
            }
        }

        private LifeTimeItem _lifetimeItem;
    }
}

