namespace Beetle23
{
    public interface IGameKitFactory
    {
        IStorage CreateStorage();
        Market CreateMarket();
    }

    public class DefaultGameKitFactory : IGameKitFactory
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