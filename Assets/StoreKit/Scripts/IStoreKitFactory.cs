public interface IStoreKitFactory
{
    IStoreStorage CreateStorage();
    Market CreateMarket();
}

public class DefaultStoreKitFactory : IStoreKitFactory
{
    public IStoreStorage CreateStorage()
    {
        return new NullableStoreStorage();
    }

    public Market CreateMarket()
    {
        return new MarketMockup();
    }
}