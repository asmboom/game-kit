using UnityEngine;
using System.Collections;
using System;

namespace Beetle23
{
    public abstract class Gate : Item
    {
        public Action OnOpened = delegate { };

    	public bool IsOpen
    	{
    		get
    		{
    			return GateStorage.IsOpen(ID);
    		}
    	}

    	public bool CanOpenNow
    	{
    		get
    		{
    			if (IsOpen)
    			{
    				return false;
    			}
    			return DoCanOpenNow();
    		}
    	}

    	public bool TryOpen()
    	{
    		if (GateStorage.IsOpen(ID))
    		{
    			return true;
    		}
    		return DoTryOpen();
    	}

		public void ForceOpen(bool open) 
		{
			if (IsOpen == open) 
			{
				return;
			}
			GateStorage.SetOpen(ID, open);
            if (open)
            {
                OnOpened();
                DoOnOpen();
            }
            else
            {
                DoOnClose();
            }
		}

    	protected abstract bool DoCanOpenNow();
		protected abstract bool DoTryOpen();
        protected abstract void DoOnOpen();
        protected abstract void DoOnClose();
    }
}
