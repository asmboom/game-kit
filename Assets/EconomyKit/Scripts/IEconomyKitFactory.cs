public interface IEconomyKitFactory
{
    IEconomyStorage CreatePrefs();
    Market CreateMarket();
}

public class DefaultStoreKitFactory : IEconomyKitFactory
{
    public IEconomyStorage CreatePrefs()
    {
        return new NullableEconomyStorage();
    }

    public Market CreateMarket()
    {
        return new MarketMockup();
    }
}