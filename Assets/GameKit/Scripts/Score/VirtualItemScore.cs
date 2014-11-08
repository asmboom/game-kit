using UnityEngine;
using System;
using System.Collections;

namespace Beetle23
{
    public class VirtualItemScore : Score
    {
    	[SerializeField]
    	public VirtualItem VirtualItem;

		protected override void PerformSaveActions()
		{
			base.PerformSaveActions();

			if (VirtualItem != null)
			{
				VirtualItem.Give((int)RuntimeScore);
			}
		}
    }
}