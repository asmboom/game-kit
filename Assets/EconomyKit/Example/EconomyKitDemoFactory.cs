﻿using UnityEngine;

public class EconomyKitDemoFactory : IEconomyKitFactory
{
    public IEconomyStorage CreatePrefs()
    {
        return new PlayerPrefsEconomyStorage();
    }

    public Market CreateMarket()
    {
#if UNITY_IPHONE
        return new MarketMockup();
        // return new MarketIOS();
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
            return new MarketMockup();
        else
            return new MarketMockup();
#else
        return new MarketMockup();
#endif
    }
}