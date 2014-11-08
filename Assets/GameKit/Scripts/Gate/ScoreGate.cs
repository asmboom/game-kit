using UnityEngine;
using System;
using System.Collections;

namespace Beetle23
{
    public class ScoreGate : Gate
    {
    	[SerializeField]
    	public Score RelatedScore;

    	[SerializeField]
    	public float RequiredScore;

		protected void OnEnable()
		{
			if (!IsOpen) 
			{
				RelatedScore.OnBeatRecord += OnBeatRecord;
			}
		}

    	protected override bool DoCanOpenNow()
    	{
    		return RelatedScore != null && 
	    		   RelatedScore.HasReachedScore(RelatedScore.Record, RequiredScore);
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
			RelatedScore.OnBeatRecord -= OnBeatRecord;
		}

		protected override void DoOnClose() 
		{
			RelatedScore.OnBeatRecord += OnBeatRecord;
		}

		private void OnBeatRecord() 
		{
			if (DoCanOpenNow()) 
			{
				ForceOpen(true);
			}
		}
    }
}
