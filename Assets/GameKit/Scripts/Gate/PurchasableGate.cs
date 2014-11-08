using UnityEngine;
using System.Collections;

namespace Beetle23
{
	public class PurchasableGate : Gate
	{
    	[SerializeField]
    	public LifeTimeItem RelatedItem;

		protected void OnEnable()
		{
			if (!IsOpen) 
			{
				RelatedItem.OnPurchased += OnPurchasedItem;
			}
		}

		protected override bool DoCanOpenNow() 
		{
			return RelatedItem != null && RelatedItem.Owned;
		}

		protected override bool DoTryOpen() 
		{
			if (CanOpenNow) 
			{
				ForceOpen(true);
				return true;
			}
			
			return false;
		}

		protected override void DoOnOpen() 
		{
			RelatedItem.OnPurchased -= OnPurchasedItem;
		}

		protected override void DoOnClose() 
		{
			RelatedItem.OnPurchased += OnPurchasedItem;
		}

		private void OnPurchasedItem() 
		{
			ForceOpen(true);
		}
	}
}

