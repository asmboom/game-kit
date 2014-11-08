using UnityEngine;
using System;
using System.Collections;

namespace Beetle23
{
    public class RangeScore : Score
    {
    	[SerializeField]
    	public float Min = 0;

    	[SerializeField]
    	public float Max = 999999;

		protected override float ClampScore(float score)
		{
			return Mathf.Clamp(score, Min, Max);
		}

		protected void OnEnable()
		{
			DefaultValue = IsHigherBetter ? Min : Max;
		}
    }
}