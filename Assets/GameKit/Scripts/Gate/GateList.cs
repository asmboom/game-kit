using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Beetle23
{
	public class GatesList : Gate
	{
		public enum GateListType
		{
			And,
			Or
		}

		[SerializeField]
		public List<Gate> Gates;

		[SerializeField]
		public GateListType Type;

		public Gate this[string id] 
		{
			get 
			{ 
				foreach(Gate gate in Gates) 
				{
					if (gate.ID == id) 
					{
						return gate;
					}
				}

				return null;
			}
		}

		public Gate this[int idx] 
		{
			get { return Gates[idx]; }
			set {  Gates[idx] = value; }
		}

		protected void OnEnable()
		{
			if (!IsOpen) 
			{
				foreach (var gate in Gates)
				{
					gate.OnOpened += OnGateOpened;
				}
			}
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

    	protected override bool DoCanOpenNow()
    	{
    		if (Type == GateListType.And)
    		{
				foreach (Gate gate in Gates) 
				{
					if (!gate.IsOpen) 
					{
						return false;
					}
				}
				return true;
			}
			else
			{
				foreach (Gate gate in Gates) 
				{
					if (gate.IsOpen) 
					{
						return true;
					}
				}
				return false;
			}
    	}

		protected override void DoOnOpen()
		{
			foreach (var gate in Gates)
			{
				gate.OnOpened -= OnGateOpened;
			}
		}

		protected override void DoOnClose()
		{
			foreach (var gate in Gates)
			{
				gate.OnOpened += OnGateOpened;
			}
		}

		private void OnGateOpened() 
		{
			if (CanOpenNow) 
			{
				ForceOpen(true);
			}
		}
	}
}