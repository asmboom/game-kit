using UnityEngine;
using System.Collections;

namespace Beetle23
{
	public class WorldGate : Gate
	{
		[SerializeField]
		public World RelatedWorld;

		protected void OnEnable()
		{
			if (!IsOpen) 
			{
				RelatedWorld.OnCompleted += OnWorldCompleted;
			}
		}

		protected override bool DoCanOpenNow() 
		{
			return RelatedWorld != null && RelatedWorld.IsCompleted;
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
			RelatedWorld.OnCompleted -= OnWorldCompleted;
		}

		protected override void DoOnClose() 
		{
			RelatedWorld.OnCompleted += OnWorldCompleted;
		}

		private void OnWorldCompleted() 
		{
			ForceOpen(true);
		}
	}
}

