public interface IEconomyKitFactory
{
    IEconomyStorage CreateStorage();
    Market CreateMarket();
}

public class DefaultEconomyKitFactory : IEconomyKitFactory
{
    public IEconomyStorage CreateStorage()
    {
        return new NullableEconomyStorage();
    }

    public Market CreateMarket()
    {
        return new MarketMockup();
    }
}