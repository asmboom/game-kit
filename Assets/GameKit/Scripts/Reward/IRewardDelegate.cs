namespace Beetle23
{
    public interface IRewardDelegate
    {
        void Give(Reward reward);
        void Take(Reward reward);
    }
}