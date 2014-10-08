namespace Beetle23
{
    public interface IEconomyKitFactory
    {
        IStorage CreateStorage();
        Market CreateMarket();
    }

    public class DefaultEconomyKitFactory : IEconomyKitFactory
    {
        public IStorage CreateStorage()
        {
            return new Storage();
        }

        public Market CreateMarket()
        {
            return new MarketMockup();
        }
    }
}