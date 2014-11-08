using UnityEngine;
using System.Collections;

namespace Beetle23
{
	public class VirtualItemGate : Gate
	{
    	[SerializeField]
    	public VirtualItem RelatedItem;

    	[SerializeField]
    	public float RequiredBalance;

		protected void OnEnable()
		{
			if (!IsOpen) 
			{
				RelatedItem.OnBalanceChanged += OnItemBalanceChanged;
			}
		}

		protected override bool DoCanOpenNow() 
		{
			return RelatedItem != null && RelatedItem.Balance >= RequiredBalance;
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
			RelatedItem.OnBalanceChanged -= OnItemBalanceChanged;
		}

		protected override void DoOnClose() 
		{
			RelatedItem.OnBalanceChanged += OnItemBalanceChanged;
		}

		private void OnItemBalanceChanged(int oldCount, int newCount) 
		{
			if (DoCanOpenNow())
			{
				ForceOpen(true);
			}
		}
	}
}

