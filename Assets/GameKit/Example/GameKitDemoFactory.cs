using UnityEngine;
using Beetle23;

public class GameKitDemoFactory : IGameKitFactory
{
    public IStorage CreateStorage()
    {
        return new Storage();
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