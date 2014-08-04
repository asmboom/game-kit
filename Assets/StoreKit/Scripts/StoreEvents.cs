using System;
using System.Collections.Generic;
using UnityEngine;

public class StoreEvents
{
    public static Action<VirtualItem> OnPurchaseStarted = delegate { };
    public static Action<VirtualItem> OnPurchaseSucceeded = delegate { };
    public static Action<VirtualItem> OnPurchaseFailed = delegate { };
}